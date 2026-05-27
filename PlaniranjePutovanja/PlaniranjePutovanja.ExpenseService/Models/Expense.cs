using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Models
{
    [DataContract]
    public class Expense
    {
        // Mora serijalizacija jer ce SF prenositi ovo kroz mrezu, jer se nalazi u ReliableDictionary
        [DataMember]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Referenca na putovanje
        /// </summary>
        [DataMember]
        public string TravelId { get; set; } = string.Empty;
        [DataMember]
        public string Name { get; set; } = string.Empty;
        [DataMember]
        public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public DateTime ExpenseDate { get; set; }
        [DataMember]
        public string Description { get; set; } = string.Empty;
        [DataMember]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [DataMember]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
