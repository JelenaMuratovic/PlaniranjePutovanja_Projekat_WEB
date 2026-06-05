using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.DTOs.Travel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public sealed class PdfGeneratorService : IPdfGeneratorService
    {
        public async Task<byte[]> GenerateTravelReportPdfAsync(
        TravelDto travel,
        IEnumerable<DestinationDto> destinations,
        IEnumerable<ActivityDto> activities,
        TravelBudgetSummaryDto budgetSummary)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                    container
                        .Page(page =>
                        {
                            page.MarginVertical(20);
                            page.MarginHorizontal(20);

                            page.Header().Text($"Izveštaj putovanja: {travel.Name}")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            page.Content().Column(column =>
                            {
                                //column.Spacing(10);
                                // Osnovni podaci putovanja
                                column.Item().Text("Osnovni podaci putovanja").Bold().FontSize(14);
                                column.Item().Text($"Period: {travel.StartDate:dd.MM.yyyy} - {travel.EndDate:dd.MM.yyyy}");
                                column.Item().Text($"Opis: {travel.Description}");
                                column.Item().PaddingBottom(10).ShowEntire();

                                // Destinacije
                                column.Item().Text("Destinacije").Bold().FontSize(14);
                                foreach (var dest in destinations)
                                {
                                    column.Item().Text($"• {dest.Name}, {dest.City}, {dest.Country} ({dest.DaysSpent} dana)");
                                }
                                column.Item().PaddingBottom(10).ShowEntire();

                                // Aktivnosti
                                column.Item().Text("Aktivnosti").Bold().FontSize(14);
                                foreach (var activity in activities)
                                {
                                    column.Item().Text($"• {activity.Name} ({activity.ActivityDate:dd.MM.yyyy}) - {activity.Price}€");
                                }
                                column.Item().PaddingBottom(10).ShowEntire(); ;

                                // Budzet
                                column.Item().Text("Budžet").Bold().FontSize(14);
                                column.Item().Text($"Planiran budžet: {budgetSummary.PlannedBudget}€");
                                column.Item().Text($"Ukupno potrošeno: {budgetSummary.TotalExpenses}€");
                                column.Item().Text($"Preostalo: {budgetSummary.RemainingBudget}€");
                                column.Item().Text($"Utrošeno: {budgetSummary.SpentPercentage:F2}%");
                            });

                            page.Footer().AlignCenter().Text($"Kreirano: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                        })
                );

                return document.GeneratePdf();
            });
        }
    }
}
