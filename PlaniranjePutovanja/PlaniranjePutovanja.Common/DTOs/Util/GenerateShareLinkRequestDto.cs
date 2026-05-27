using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Util
{
    public class GenerateShareLinkRequestDto
    {
        // public string TravelId { get; set; } = string.Empty;
        public ShareAccessLevel AccessLevel { get; set; } = ShareAccessLevel.View;
        public int ExpirationDays { get; set; } = 7; // Da li token istice za nedelju dana
    }
}
