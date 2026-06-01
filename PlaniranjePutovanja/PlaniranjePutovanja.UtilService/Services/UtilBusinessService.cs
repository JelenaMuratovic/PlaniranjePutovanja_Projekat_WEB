using PlaniranjePutovanja.Common.DTOs.Util;
using PlaniranjePutovanja.UtilService.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Services
{
    public sealed class UtilBusinessService : IUtilBusinessService
    {
        private readonly ITravelServiceClient _travelServiceClient;
        private readonly IExpenseServiceClient _expenseServiceClient;
        private readonly IPdfGeneratorService _pdfGeneratorService;
        private readonly IQrCodeGeneratorService _qrCodeGeneratorService;
        private readonly IShareTokenService _shareTokenService;

        public UtilBusinessService(
            ITravelServiceClient travelServiceClient,
            IExpenseServiceClient expenseServiceClient,
            IPdfGeneratorService pdfGeneratorService,
            IQrCodeGeneratorService qrCodeGeneratorService,
            IShareTokenService shareTokenService)
        {
            _travelServiceClient = travelServiceClient;
            _expenseServiceClient = expenseServiceClient;
            _pdfGeneratorService = pdfGeneratorService;
            _qrCodeGeneratorService = qrCodeGeneratorService;
            _shareTokenService = shareTokenService;
        }

        public async Task<GeneratedFileDto> GenerateTravelPlanPdfAsync(string travelId)
        {
            // Povucemo sve potrebne podatke iz ostalih servisa
            var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            var destinations = await _travelServiceClient.GetDestinationsByTravelIdAsync(travelId);
            var activities = await _travelServiceClient.GetActivitiesByTravelIdAsync(travelId);
            var budgetSummary = await _expenseServiceClient.GetBudgetSummaryAsync(travelId);

            // Generisemo PDF
            var pdfBytes = await _pdfGeneratorService.GenerateTravelReportPdfAsync(
                travel,
                destinations,
                activities,
                budgetSummary ?? throw new InvalidOperationException("Budget summary not found."));

            return new GeneratedFileDto
            {
                FileContents = pdfBytes,
                ContentType = "application/pdf",
                FileName = $"{travel.Name}_plan_{DateTime.Now:yyyyMMdd}.pdf"
            };
        }

        public async Task<ShareLinkResponseDto> GenerateShareQrCodeAsync(string travelId,GenerateShareLinkRequestDto request)
        {
            // Validiraj da putovanje postoji
            var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            // Generise JWT za deljenje
            var shareToken = await _shareTokenService.GenerateShareTokenAsync(
                travelId,
                request.AccessLevel,
                request.ExpirationDays);

            // URL koji ce biti u QR kodu (naprimer localhost ili production URL)
            var targetUrl = $"http://localhost:8081/api/util/shared?token={shareToken}";

            // Generise QR kod
            var qrCodeImage = await _qrCodeGeneratorService.GenerateQrCodeAsync(targetUrl);

            return new ShareLinkResponseDto
            {
                Token = shareToken,
                TargetUrl = targetUrl,
                QrCodeImage = new GeneratedFileDto
                {
                    FileContents = qrCodeImage,
                    ContentType = "image/png",
                    FileName = $"share_qr_{travelId}.png"
                }
            };
        }
    }
}
