using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using efilling_api.Models;
using Microsoft.EntityFrameworkCore;

namespace efilling_api.Services
{
    public class FilingService
    {
        private readonly HttpClient _httpClient;
        private readonly EFilling_DBContext _context;
        private readonly IConfiguration _config;
        //private readonly ILogger<FilingScheduler> _logger;

        public FilingService(HttpClient httpClient, EFilling_DBContext context, IConfiguration config)
        {
            _httpClient = httpClient;
            _context = context;
            _config = config;
        }

        public async Task FetchAndStoreFilingsAsync()
        {
            string apiUrl = _config["ApiSettings:BaseUrl"] + "GetFilingListAll";

            // Create HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["ApiSettings:AuthToken"]);

            try
            {
                var responseMessage = await _httpClient.SendAsync(request);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    //_logger.LogError("Failed to fetch filings. Status Code: {StatusCode}, URL: {Url}, Content: {Content}",
                    //    responseMessage.StatusCode, apiUrl, await responseMessage.Content.ReadAsStringAsync());
                    return; // Stop execution if request failed
                }

                var response = await responseMessage.Content.ReadFromJsonAsync<MatchingFilingRequest>();

                if (response?.success == true && response.data?.MatchingFiling != null)
                {
                    foreach (var filing in response.data.MatchingFiling)
                    {
                        string filingId = filing.DocumentIdentification?.FirstOrDefault(x => x.Item.Value == "FILINGID")?.IdentificationID?.Value;

                        if (string.IsNullOrEmpty(filingId))
                        {
                            //_logger.LogWarning("Skipping filing due to missing FILINGID.");
                            continue;
                        }

                        var existingFiling = await _context.Filinglists.FirstOrDefaultAsync(f => f.FILINGID == filingId);

                        if (existingFiling != null)
                        {
                            // Update existing record
                            existingFiling.CaseTitle = filing.CaseTitle?.Value;
                            existingFiling.CaseNumber = filing.CaseNumber?.Value;
                            existingFiling.CaseJudge = filing.CaseJudge?.Value;
                            existingFiling.FilingType = filing.FilingType?.Value;
                            existingFiling.FilingAttorney = filing.FilingAttorney?.Value;
                            existingFiling.FilingCode = filing.FilingCode?.Value;
                            existingFiling.OrganizationIdentificationID = filing.OrganizationIdentificationID?.Value;
                            existingFiling.CaseCategoryCode = filing.CaseCategoryCode?.Value;
                            existingFiling.CaseTypeCode = filing.CaseTypeCode?.Value;
                            existingFiling.DocumentDescriptionText = filing.DocumentDescriptionText?.Value;
                            existingFiling.DocumentFileControlID = filing.DocumentFileControlID?.Value;
                            existingFiling.DocumentFiledDate = filing.DocumentFiledDate?.Item?.Value;
                            existingFiling.DocumentReceivedDate = filing.DocumentReceivedDate?.Item?.Value;
                            existingFiling.DocumentSubmitterName = filing.DocumentSubmitter?.Item?.PersonName?.PersonFullName?.Value;
                            existingFiling.DocumentSubmitterID = filing.DocumentSubmitter?.Item?.PersonOtherIdentification?.FirstOrDefault()?.IdentificationID?.Value;
                            existingFiling.CaseTrackingID = filing.CaseTrackingID?.Value;
                            existingFiling.FilingStatusCode = filing.FilingStatus?.FilingStatusCode;
                            existingFiling.StatusDescriptionText = filing.FilingStatus?.StatusDescriptionText?.FirstOrDefault()?.Value;
                            existingFiling.ENVELOPEID = filing.DocumentIdentification?.FirstOrDefault(x => x.Item.Value == "ENVELOPEID")?.IdentificationID?.Value;
                            

                            _context.Filinglists.Update(existingFiling);
                            //_logger.LogInformation("Updated existing filing with ID: {FilingId}", filingId);
                        }
                        else
                        {
                            // Insert new record
                            var newFiling = new Filinglist
                            {
                                CaseTitle = filing.CaseTitle?.Value,
                                CaseNumber = filing.CaseNumber?.Value,
                                CaseJudge = filing.CaseJudge?.Value,
                                FilingType = filing.FilingType?.Value,
                                FilingAttorney = filing.FilingAttorney?.Value,
                                FilingCode = filing.FilingCode?.Value,
                                OrganizationIdentificationID = filing.OrganizationIdentificationID?.Value,
                                CaseCategoryCode = filing.CaseCategoryCode?.Value,
                                CaseTypeCode = filing.CaseTypeCode?.Value,
                                DocumentDescriptionText = filing.DocumentDescriptionText?.Value,
                                DocumentFileControlID = filing.DocumentFileControlID?.Value,
                                DocumentFiledDate = filing.DocumentFiledDate?.Item?.Value,
                                DocumentReceivedDate = filing.DocumentReceivedDate?.Item?.Value,
                                DocumentSubmitterName = filing.DocumentSubmitter?.Item?.PersonName?.PersonFullName?.Value,
                                DocumentSubmitterID = filing.DocumentSubmitter?.Item?.PersonOtherIdentification?.FirstOrDefault()?.IdentificationID?.Value,
                                CaseTrackingID = filing.CaseTrackingID?.Value,
                                FilingStatusCode = filing.FilingStatus?.FilingStatusCode,
                                StatusDescriptionText = filing.FilingStatus?.StatusDescriptionText?.FirstOrDefault()?.Value,
                                ENVELOPEID = filing.DocumentIdentification?.FirstOrDefault(x => x.Item.Value == "ENVELOPEID")?.IdentificationID?.Value,
                                FILINGID = filingId
                            };

                            _context.Filinglists.Add(newFiling);
                            //_logger.LogInformation("Inserted new filing with ID: {FilingId}", filingId);
                        }
                    }

                    await _context.SaveChangesAsync();
                    //_logger.LogInformation("Successfully fetched and stored filings at {Time}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while fetching filings.");
            }
        }


    }
}
