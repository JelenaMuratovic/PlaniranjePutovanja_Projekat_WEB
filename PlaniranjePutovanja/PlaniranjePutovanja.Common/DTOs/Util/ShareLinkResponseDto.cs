using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Util
{
    public class ShareLinkResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public GeneratedFileDto QrCodeImage { get; set; } = new GeneratedFileDto();
    }
}
