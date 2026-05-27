using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Models
{
    public class Activity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Referenca na destinaciju kojoj aktivnost pripada
        /// </summary>
        public string DestinationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public decimal Price { get; set; }
        public ActivityStatus Status { get; set; } = ActivityStatus.Planned;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation property
        public Destination? Destination { get; set; }
    }
}
