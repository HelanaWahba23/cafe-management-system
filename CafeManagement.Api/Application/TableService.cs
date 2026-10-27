using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CafeManagement.Api.Application;

public sealed class TableService(IDbContextFactory<CafeDbContext> factory, StaffAccess staff,
    IOptions<PublicMenuOptions> options)
{
    private readonly Uri publicOrigin = new(options.Value.PublicWebBaseUrl, UriKind.Absolute);

    public Task<(TableResponse[]? Value, OperationError? Error)> ListAsync(ClaimsPrincipal principal,
        CancellationToken cancellationToken = default) => ExecuteAsync<TableResponse[]>(principal, async db =>
    {
        var rows = await db.CafeTables.AsNoTracking().OrderBy(t => t.Id).ToArrayAsync(cancellationToken);
        return (rows.Select(Response).ToArray(), null);
    }, cancellationToken);

    public Task<(TableResponse? Value, OperationError? Error)> GetAsync(ClaimsPrincipal principal, int id,
        CancellationToken cancellationToken = default) => ExecuteAsync<TableResponse>(principal, async db =>
    {
        if (id <= 0) return (null, InvalidId());
        var table = await db.CafeTables.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
        return table is null ? (null, Missing()) : (Response(table), null);
    }, cancellationToken);

    public Task<(TableResponse? Value, OperationError? Error)> CreateAsync(ClaimsPrincipal principal,
        TableEditModel input, CancellationToken cancellationToken = default) => SaveAsync(principal, null, input, cancellationToken);

    public Task<(TableResponse? Value, OperationError? Error)> EditAsync(ClaimsPrincipal principal, int id,
        TableEditModel input, CancellationToken cancellationToken = default) => SaveAsync(principal, id, input, cancellationToken);

    private Task<(TableResponse? Value, OperationError? Error)> SaveAsync(ClaimsPrincipal principal,
        int? id, TableEditModel input, CancellationToken cancellationToken) => ExecuteAsync<TableResponse>(principal, async db =>
    {
        if (id is <= 0) return (null, InvalidId());
        var validation = new List<ValidationResult>();
        if (!Validator.TryValidateObject(input, new ValidationContext(input), validation, true))
            return (null, new OperationError(OperationErrorKind.Invalid, "Check the supplied values.",
                FieldErrors: new Dictionary<string, string[]> { ["label"] = validation.Select(v => v.ErrorMessage!).ToArray() }));

        var label = input.Label.Trim();
        var normalized = label.ToUpperInvariant();
        var existing = id is null ? null : await db.CafeTables.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (id is not null && existing is null) return (null, Missing());
        if (await db.CafeTables.AnyAsync(t => t.NormalizedLabel == normalized && (!id.HasValue || t.Id != id.Value), cancellationToken))
            return (null, DuplicateLabel());

        // Only a proven GUID-index collision retries creation. Never replay an uncertain write.
        for (var attempt = 0; attempt < 3; attempt++)
        {
            var table = existing ?? new CafeTable { Label = label, NormalizedLabel = normalized };
            table.Label = label;
            table.NormalizedLabel = normalized;
            if (id is null) db.CafeTables.Add(table);
            try
            {
                await db.SaveChangesAsync(cancellationToken);
                return (Response(table), null);
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception, "IX_CafeTables_NormalizedLabel"))
            {
                return (null, DuplicateLabel());
            }
            catch (DbUpdateException exception) when (id is null && IsUniqueViolation(exception, "IX_CafeTables_PublicCode"))
            {
                // Discard the failed new entity; its replacement generates a fresh random GUID.
                db.ChangeTracker.Clear();
            }
        }
        return (null, Unexpected());
    }, cancellationToken);

    private TableResponse Response(CafeTable table) => new(table.Id, table.Label, table.PublicCode,
        new Uri(publicOrigin, $"/menu/t/{table.PublicCode:D}").AbsoluteUri);

    private async Task<(T? Value, OperationError? Error)> ExecuteAsync<T>(ClaimsPrincipal principal,
        Func<CafeDbContext, Task<(T? Value, OperationError? Error)>> operation, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var access = await staff.CheckAdminAsync(principal);
            if (access != StaffAccessResult.Allowed)
                return (null, new OperationError(access == StaffAccessResult.Unauthenticated
                    ? OperationErrorKind.Unauthenticated : OperationErrorKind.Forbidden, "Administrator access is required."));
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            return await operation(db);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception) { return (null, Unexpected()); }
    }

    private static bool IsUniqueViolation(DbUpdateException exception, string index) =>
        exception.InnerException is SqlException sql && sql.Errors.Cast<SqlError>()
            .Any(error => error.Number is 2601 or 2627 && error.Message.Contains(index, StringComparison.Ordinal));

    private static OperationError DuplicateLabel() => new(OperationErrorKind.Conflict,
        "A table with this label already exists.", "duplicate_table_label", "label",
        new Dictionary<string, string[]> { ["label"] = ["Choose a unique table label."] });
    private static OperationError InvalidId() => new(OperationErrorKind.Invalid, "Use a positive record ID.");
    private static OperationError Missing() => new(OperationErrorKind.NotFound, "The requested table was not found.");
    private static OperationError Unexpected() => new(OperationErrorKind.Unexpected, "Unable to complete the table operation.");
}
