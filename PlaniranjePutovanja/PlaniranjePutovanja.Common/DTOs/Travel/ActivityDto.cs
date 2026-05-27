using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Travel
{
    public sealed class ActivityDto
    {
        public string Id { get; set; } = string.Empty;
        public string DestinationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = ActivityStatus.Planned.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
