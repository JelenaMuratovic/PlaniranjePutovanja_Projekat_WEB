using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Models
{
    public class Destination
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Referenca na putovanje kojem pripada ova destinacija
        /// </summary>
        public string TravelId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        /// <summary>
        /// Za mapu koordinate
        /// </summary>
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int DaysSpent { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation property
        public Travel? Travel { get; set; }
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
