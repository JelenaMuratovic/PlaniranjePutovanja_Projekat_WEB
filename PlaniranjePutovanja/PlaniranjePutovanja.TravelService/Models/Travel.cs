using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Models
{
    public class Travel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Referenca na korisnika kojem pripada ova destinacija
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        // Navigation property
        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
        public ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();

    }
}
