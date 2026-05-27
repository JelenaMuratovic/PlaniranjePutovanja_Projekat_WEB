using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Util
{
    public class GeneratedFileDto
    {
        public byte[] FileContents { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty; // Npr. "application/pdf" ili "image/png"
        public string FileName { get; set; } = string.Empty;
    }
}
