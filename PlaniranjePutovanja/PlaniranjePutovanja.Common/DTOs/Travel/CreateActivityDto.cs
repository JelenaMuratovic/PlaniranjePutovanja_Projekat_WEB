using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Travel
{
    public sealed class CreateActivityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public decimal Price { get; set; }
        public ActivityStatus Status { get; set; } = ActivityStatus.Planned;
    }
}
