using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace CafeManagement.Api.Application;

public sealed class OrderService(IDbContextFactory<CafeDbContext> factory)
{
    public Task<OrderPlacementOutcome> PlaceAsync(string? capability, OrderPlacementInput? input,
        CancellationToken cancellationToken = default) => PlaceCoreAsync(capability, input, null, cancellationToken);

    internal Task<OrderPlacementOutcome> PlaceCounterAsync(string? attempt, OrderPlacementInput? input,
        string creator, CancellationToken ct) => PlaceCoreAsync(attempt, input, creator, ct);

    private async Task<OrderPlacementOutcome> PlaceCoreAsync(string? capability, OrderPlacementInput? input,
        string? creator, CancellationToken cancellationToken)
    {
        var error = Prepare(capability, input?.Draft, out var draft, creator);
        if (error is not null) return new(null, error);
        // Snapshot the comparison vector before the first await, just like the immutable intent.
        var prices = input!.ReviewedPrices?.Select(p => p is null ? null : new ReviewedPrice
            { ProductId = p.ProductId, UnitPrice = p.UnitPrice }).ToArray();
        var validation = new OrderPlacementInput
        {
            Draft = new OrderDraft
            {
                OrderType = draft!.Type.ToString(), TablePublicCode = draft.TablePublicCode,
                CustomerName = draft.CustomerName, CustomerPhone = draft.CustomerPhone,
                Items = draft.Items.Select(i => new OrderDraftItem { ProductId = i.ProductId, Quantity = i.Quantity }).ToArray()
            },
            ReviewedPrices = prices
        };
        var issues = (creator is null ? validation.Validate(new ValidationContext(validation)) : validation.ValidateCounter()).ToArray();
        if (issues.Length > 0)
            return new(null, new OperationError(OperationErrorKind.Invalid, "Review the selected product prices.",
                FieldErrors: issues.SelectMany(issue => issue.MemberNames.Select(field =>
                    (Field: field, Message: issue.ErrorMessage!))).GroupBy(issue => issue.Field)
                    .ToDictionary(group => group.Key, group => group.Select(issue => issue.Message).Distinct().ToArray())));

        try
        {
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
            var existing = await FindAsync(db, draft!.CapabilityHash, cancellationToken, creator);
            if (existing is not null) return Replay(existing, draft.IntentHash);

            // Table/product reads retain their locks only through this short local acceptance.
            var review = await ReviewCoreAsync(db, draft, cancellationToken);
            if (review.Error is not null) return new(null, review.Error);
            var comparison = prices!.ToDictionary(p => p!.ProductId, p => { p!.TryReadUnitPrice(out var value); return value; });
            if (review.Value!.Items.Any(item => comparison[item.ProductId] != ReadMoney(item.UnitPrice)))
                return new(null, new OperationError(OperationErrorKind.Conflict,
                    "Prices have changed. Review the updated amounts and confirm again.", "price_changed"), Review: review.Value);

            int? tableId = draft.Type == OrderType.DineIn
                ? await db.CafeTables.Where(t => t.PublicCode == draft.TablePublicCode).Select(t => (int?)t.Id)
                    .SingleAsync(cancellationToken) : null;
            var order = new Order
            {
                CapabilityHash = draft.CapabilityHash, IntentHash = draft.IntentHash, Type = draft.Type,
                TableId = tableId, TableLabel = review.Value.Table?.Label,
                Origin = creator is null ? OrderOrigin.Customer : OrderOrigin.Counter, CreatedByUserId = creator,
                CustomerName = string.IsNullOrEmpty(draft.CustomerName) ? null : draft.CustomerName,
                CustomerPhone = string.IsNullOrEmpty(draft.CustomerPhone) ? null : draft.CustomerPhone,
                Total = ReadMoney(review.Value.Total)
            };
            foreach (var item in review.Value.Items)
                order.Lines.Add(new OrderLine
                {
                    ProductId = item.ProductId, ProductName = item.ProductName, Quantity = item.Quantity,
                    UnitPrice = ReadMoney(item.UnitPrice), LineTotal = ReadMoney(item.LineTotal)
                });
            if (order.Lines.Count == 0 || order.Lines.Sum(line => line.LineTotal) != order.Total)
                return new(null, InvalidAmount());
            db.Orders.Add(order);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new(new OrderPlacementResult(Project(order)), null, Created: true);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql
            && sql.Errors.Cast<SqlError>().Any(e => e.Number is 2601 or 2627
                && e.Message.Contains("IX_Orders_CapabilityHash", StringComparison.Ordinal)))
        {
            // The failed transaction/context have been disposed. Only this precise unique index
            // permits winner recovery; unrelated unique failures must never claim success.
            try
            {
                await using var recovery = await factory.CreateDbContextAsync(cancellationToken);
                var winner = await FindAsync(recovery, draft!.CapabilityHash, cancellationToken, creator);
                return winner is null ? Uncertain() : Replay(winner, draft.IntentHash);
            }
            catch (Exception) { return Uncertain(); }
        }
        catch (OverflowException) { return new(null, InvalidAmount()); }
        catch (Exception)
        {
            // Cancellation/timeout/commit loss may be ambiguous. Never retry writes or replace
            // the credential here; an explicit capability-only read is the recovery operation.
            return Uncertain();
        }
    }

    public async Task<(OrderTracking? Value, OperationError? Error)> TrackAsync(string? capability,
        CancellationToken cancellationToken = default)
    {
        if (!TryHashCapability(capability, out var hash)) return (null, MissingTracking());
        try
        {
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            var order = await FindAsync(db, hash!, cancellationToken);
            return order is null ? (null, MissingTracking()) : (Project(order), null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception)
        {
            return (null, new OperationError(OperationErrorKind.Unexpected,
                "Unable to check the order outcome. Please try again."));
        }
    }

    private static Task<Order?> FindAsync(CafeDbContext db, byte[] hash, CancellationToken cancellationToken, string? creator = null) =>
        db.Orders.AsNoTracking().Include(order => order.Lines).SingleOrDefaultAsync(order => order.CapabilityHash == hash
            && (creator == null ? order.Origin == OrderOrigin.Customer : order.Origin == OrderOrigin.Counter && order.CreatedByUserId == creator), cancellationToken);

    private static OrderPlacementOutcome Replay(Order existing, byte[] intent) =>
        CryptographicOperations.FixedTimeEquals(existing.IntentHash, intent)
            ? new(new OrderPlacementResult(Project(existing)), null)
            : new(null, new OperationError(OperationErrorKind.Conflict,
                "This private order context has already been used for different order details.", "submission_conflict"));

    private static OrderPlacementOutcome Uncertain() => new(null, new OperationError(OperationErrorKind.Unexpected,
        "The order outcome is uncertain. Keep this private context and check the order outcome before retrying.", "outcome_uncertain"));
    private static OperationError MissingTracking() => new(OperationErrorKind.NotFound,
        "This private order could not be found. Check your private link.", "order_not_found");
    private static decimal ReadMoney(string value) => decimal.Parse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
    private static OrderTracking Project(Order order) => new(order.Reference, order.Type.ToString(), order.TableLabel,
        order.Lines.OrderBy(line => line.ProductId).Select(line => new OrderTrackingItem(line.ProductName, line.Quantity,
            Money(line.UnitPrice), Money(line.LineTotal))).ToArray(), Money(order.Total), order.Status.ToString(),
        order.RejectionReason, order.CreatedAtUtc);

    public Task<(OrderReview? Value, OperationError? Error)> ReviewAsync(string? capability,
        OrderDraft? input, CancellationToken cancellationToken = default) => ReviewCoreEntryAsync(capability, input, null, cancellationToken);

    internal Task<(OrderReview? Value, OperationError? Error)> ReviewCounterAsync(string? attempt,
        OrderDraft? input, string creator, CancellationToken ct) => ReviewCoreEntryAsync(attempt, input, creator, ct);

    private async Task<(OrderReview? Value, OperationError? Error)> ReviewCoreEntryAsync(string? capability,
        OrderDraft? input, string? creator, CancellationToken cancellationToken)
    {
        var error = Prepare(capability, input, out var draft, creator);
        if (error is not null) return (null, error);

        try
        {
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            return await ReviewCoreAsync(db, draft!, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (OverflowException) { return (null, InvalidAmount()); }
        catch (Exception)
        {
            // No raw exception, credential, contact data or draft is logged or returned.
            return (null, new OperationError(OperationErrorKind.Unexpected,
                "Unable to review the order. Please try again."));
        }
    }

    private static async Task<(OrderReview? Value, OperationError? Error)> ReviewCoreAsync(
        CafeDbContext db, PreparedDraft draft, CancellationToken cancellationToken)
    {
        OrderReviewTable? table = null;
        if (draft!.Type == OrderType.DineIn)
        {
            table = await db.CafeTables.AsNoTracking().Where(t => t.PublicCode == draft.TablePublicCode)
                .Select(t => new OrderReviewTable(t.PublicCode, t.Label)).SingleOrDefaultAsync(cancellationToken);
            if (table is null)
                return (null, new OperationError(OperationErrorKind.NotFound,
                    "This table could not be found. Check the table QR code.", "table_not_found"));
        }

        var ids = draft!.Items.Select(item => item.ProductId).ToArray();
        var products = await db.Products.AsNoTracking().Where(p => ids.Contains(p.Id)).OrderBy(p => p.Id)
            .Select(p => new { p.Id, p.Name, p.Price, p.IsAvailable }).ToArrayAsync(cancellationToken);
        if (products.Length != ids.Length)
            return (null, new OperationError(OperationErrorKind.NotFound,
                "A selected product could not be found. Reload the menu.", "product_not_found"));
        if (products.Any(p => !p.IsAvailable))
            return (null, new OperationError(OperationErrorKind.Conflict,
                $"Selected products are no longer available (product IDs: {string.Join(", ", products.Where(p => !p.IsAvailable).Select(p => p.Id))}). Update the cart.",
                "product_unavailable", "Items"));

        var items = new List<OrderReviewItem>();
        decimal total = 0;
        foreach (var product in products)
        {
            var quantity = draft.Items.Single(item => item.ProductId == product.Id).Quantity;
            var lineTotal = checked(product.Price * quantity);
            total = checked(total + lineTotal);
            if (!ValidMoney(product.Price) || !ValidMoney(lineTotal) || !ValidMoney(total))
                return (null, InvalidAmount());
            items.Add(new OrderReviewItem(product.Id, product.Name, quantity, Money(product.Price), Money(lineTotal)));
        }
        return (new OrderReview(draft.Type.ToString(), table, items.ToArray(), Money(total)), null);
    }

    private static OperationError? Prepare(string? capability, OrderDraft? input, out PreparedDraft? prepared, string? creator = null)
    {
        prepared = null;
        if (!TryHashCapability(capability, out var capabilityHash))
            return new OperationError(OperationErrorKind.Invalid, "Supply a valid private order context.",
                "invalid_capability");
        if (input is null)
            return new OperationError(OperationErrorKind.Invalid, "Supply the order draft.",
                FieldErrors: new Dictionary<string, string[]> { ["Draft"] = ["Supply the order draft."] });

        // Copy mutable caller DTOs before any await; validation and hashing use the same snapshot.
        var snapshot = new OrderDraft
        {
            OrderType = input.OrderType, TablePublicCode = input.TablePublicCode,
            CustomerName = input.CustomerName, CustomerPhone = input.CustomerPhone,
            Items = input.Items?.Select(item => item is null ? null : new OrderDraftItem
                { ProductId = item.ProductId, Quantity = item.Quantity }).ToArray()
        };
        var issues = (creator is null ? snapshot.Validate(new ValidationContext(snapshot)) : snapshot.ValidateCounter()).ToArray();
        if (issues.Length > 0)
            return new OperationError(OperationErrorKind.Invalid, "Check the supplied order details.",
                FieldErrors: issues.SelectMany(issue => issue.MemberNames.Select(field =>
                    (Field: field, Message: issue.ErrorMessage!))).GroupBy(issue => issue.Field)
                    .ToDictionary(group => group.Key, group => group.Select(issue => issue.Message).Distinct().ToArray()));

        var items = snapshot.Items!.Select(item => new DraftSelection(item!.ProductId, item.Quantity))
            .OrderBy(item => item.ProductId).ToArray();
        var type = Enum.Parse<OrderType>(snapshot.OrderType);
        if (creator is not null)
        {
            using var counterBuffer = new MemoryStream();
            using (var writer = new BinaryWriter(counterBuffer, Encoding.UTF8, leaveOpen: true))
            { writer.Write("CafeManagement.CounterAttempt.v1"); writer.Write(creator); writer.Write(capabilityHash!); }
            capabilityHash = SHA256.HashData(counterBuffer.GetBuffer().AsSpan(0, (int)counterBuffer.Length));
            CryptographicOperations.ZeroMemory(counterBuffer.GetBuffer());
        }
        prepared = new PreparedDraft(type, snapshot.TablePublicCode, snapshot.CustomerName, snapshot.CustomerPhone,
            items, capabilityHash!, HashIntent(type, snapshot.TablePublicCode, snapshot.CustomerName, snapshot.CustomerPhone, items));
        return null;
    }

    private static bool TryHashCapability(string? capability, out byte[]? hash)
    {
        hash = null;
        if (capability is null || capability.Length != 43
            || capability.Any(c => !(c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '-' or '_')))
            return false;
        Span<byte> bytes = stackalloc byte[32];
        if (!Convert.TryFromBase64String(capability.Replace('-', '+').Replace('_', '/') + "=", bytes, out var length)
            || length != 32)
            return false;
        // Reject noncanonical encodings with nonzero unused bits in the final character.
        if (Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_') != capability)
            return false;
        hash = SHA256.HashData(bytes);
        CryptographicOperations.ZeroMemory(bytes);
        return true;
    }

    private static byte[] HashIntent(OrderType type, Guid? table, string name, string phone, DraftSelection[] items)
    {
        using var buffer = new MemoryStream();
        using (var writer = new BinaryWriter(buffer, Encoding.UTF8, leaveOpen: true))
        {
            // BinaryWriter's strings are UTF-8 byte-length-prefixed; integers are little endian.
            // Version and fixed field order distinguish this format from later formats.
            writer.Write("CafeManagement.OrderIntent.v1");
            writer.Write(type.ToString());
            writer.Write(table?.ToString("D") ?? string.Empty);
            writer.Write(name);
            writer.Write(phone);
            writer.Write(items.Length);
            foreach (var item in items)
            {
                writer.Write(item.ProductId);
                writer.Write(item.Quantity);
            }
        }
        try { return SHA256.HashData(buffer.GetBuffer().AsSpan(0, checked((int)buffer.Length))); }
        finally { CryptographicOperations.ZeroMemory(buffer.GetBuffer()); }
    }

    private static bool ValidMoney(decimal value) => value > 0 && value <= ProductEditModel.MaximumPrice
        && decimal.Round(value, 2) == value;
    private static string Money(decimal value) => value.ToString("F2", CultureInfo.InvariantCulture);
    private static OperationError InvalidAmount() => new(OperationErrorKind.Invalid,
        "The selected quantities or amounts exceed the supported monetary range.", "invalid_amount", "Items");

    private sealed record DraftSelection(int ProductId, int Quantity);
    private sealed record PreparedDraft(OrderType Type, Guid? TablePublicCode, string CustomerName, string CustomerPhone,
        DraftSelection[] Items, byte[] CapabilityHash, byte[] IntentHash);
}
