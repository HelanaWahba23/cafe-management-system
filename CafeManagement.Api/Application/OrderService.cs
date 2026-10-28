using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

public sealed class OrderService(IDbContextFactory<CafeDbContext> factory)
{
    public async Task<(OrderReview? Value, OperationError? Error)> ReviewAsync(string? capability,
        OrderDraft? input, CancellationToken cancellationToken = default)
    {
        var error = Prepare(capability, input, out var draft);
        if (error is not null) return (null, error);

        try
        {
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
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
                    "A selected product is no longer available. Update the cart.", "product_unavailable"));

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
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (OverflowException) { return (null, InvalidAmount()); }
        catch (Exception)
        {
            // No raw exception, credential, contact data or draft is logged or returned.
            return (null, new OperationError(OperationErrorKind.Unexpected,
                "Unable to review the order. Please try again."));
        }
    }

    private static OperationError? Prepare(string? capability, OrderDraft? input, out PreparedDraft? prepared)
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
        var issues = snapshot.Validate(new ValidationContext(snapshot)).ToArray();
        if (issues.Length > 0)
            return new OperationError(OperationErrorKind.Invalid, "Check the supplied order details.",
                FieldErrors: issues.SelectMany(issue => issue.MemberNames.Select(field =>
                    (Field: field, Message: issue.ErrorMessage!))).GroupBy(issue => issue.Field)
                    .ToDictionary(group => group.Key, group => group.Select(issue => issue.Message).Distinct().ToArray()));

        var items = snapshot.Items!.Select(item => new DraftSelection(item!.ProductId, item.Quantity))
            .OrderBy(item => item.ProductId).ToArray();
        var type = Enum.Parse<OrderType>(snapshot.OrderType);
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
