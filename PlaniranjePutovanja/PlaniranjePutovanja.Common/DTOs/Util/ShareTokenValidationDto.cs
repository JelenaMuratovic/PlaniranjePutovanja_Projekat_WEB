using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Util
{
    public sealed class ShareTokenValidationDto
    {
        public string TravelId { get; set; } = string.Empty;

        public ShareAccessLevel AccessLevel { get; set; }
    }
}
