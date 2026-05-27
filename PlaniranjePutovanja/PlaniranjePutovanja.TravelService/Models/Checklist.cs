using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Models
{
    public class Checklist
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Referenca na putovanje kojem pripada
        /// </summary>
        public string TravelId { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; }
        // Navigation property
        public Travel? Travel { get; set; }
    }
}
