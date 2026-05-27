using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Travel
{
    public sealed class ChecklistDto
    {
        public string Id { get; set; } = string.Empty;
        public string TravelId { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
