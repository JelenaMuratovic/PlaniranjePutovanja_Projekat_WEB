using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public sealed class QrCodeGeneratorService : IQrCodeGeneratorService
    {
        public async Task<byte[]> GenerateQrCodeAsync(string data)
        {
            return await Task.Run(() =>
            {
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        var qrCodeImage = qrCode.GetGraphic(10); // 10 = velicina piksela po QR modulu
                        return qrCodeImage;
                    }
                }
            });
        }
    }
}
