using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public interface IPdfGeneratorService
    {
        /// <summary>
        /// Generise PDF izvestaj sa detaljima putovanja, destinacija, aktivnosti i troskova
        /// </summary>
        Task<byte[]> GenerateTravelReportPdfAsync(
            TravelDto travel,
            IEnumerable<DestinationDto> destinations,
            IEnumerable<ActivityDto> activities,
            TravelBudgetSummaryDto budgetSummary);
    }
}
