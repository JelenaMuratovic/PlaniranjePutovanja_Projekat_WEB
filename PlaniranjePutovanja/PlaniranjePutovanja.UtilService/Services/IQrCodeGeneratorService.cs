using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public interface IQrCodeGeneratorService
    {
        /// <summary>
        /// Generise QR kod iz URL-a ili teksta i vraca kao PNG sliku (bajt niz)
        /// </summary>
        Task<byte[]> GenerateQrCodeAsync(string data);
    }
}
