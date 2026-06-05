using PlaniranjePutovanja.Common.DTOs.Travel;
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

            // URL vodi na novu React stranicu
            var targetUrl = $"http://localhost:5173/shared-travel?token={shareToken}";

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

        public async Task<TravelDto> GetSharedTravelByTokenAsync(string token)
        {
            var tokenData = await _shareTokenService.ValidateShareTokenAsync(token);
            if (tokenData == null)
            {
                return null; // Token nije validan ili je istekao
            }

            var travelId = tokenData.Value.TravelId;

            var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
            return travel;
        }

        public async Task<ShareTokenValidationDto?> ValidateShareTokenAsync(string token)
        {
            var tokenData = await _shareTokenService.ValidateShareTokenAsync(token);

            if (tokenData == null)
            {
                return null;
            }

            return new ShareTokenValidationDto
            {
                TravelId = tokenData.Value.TravelId,
                AccessLevel = tokenData.Value.AccessLevel
            };
        }
    }
}
