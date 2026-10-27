using System.Security.Claims;
using QRCoder;

namespace CafeManagement.Api.Application;

public sealed class TableQrService(TableService tables)
{
    public async Task<(byte[]? Value, OperationError? Error)> GetPngAsync(ClaimsPrincipal principal, int id,
        CancellationToken cancellationToken = default)
    {
        // GetAsync enforces the same current-user/stamp/Admin check as every table operation.
        var result = await tables.GetAsync(principal, id, cancellationToken);
        if (result.Error is not null) return (null, result.Error);
        try
        {
            using var data = QRCodeGenerator.GenerateQrCode(result.Value!.MenuUrl, QRCodeGenerator.ECCLevel.Q);
            using var png = new PngByteQRCode(data);
            return (png.GetGraphic(8), null);
        }
        catch (Exception)
        {
            return (null, new OperationError(OperationErrorKind.Unexpected, "Unable to generate the table QR code."));
        }
    }
}
