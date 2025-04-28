using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    [Table("filinglist")]
    public class Filinglist
    {
        [Key]
        public int Id { get; set; }
        public string? CaseTitle { get; set; }
        public string? CaseNumber { get; set; }
        public string? CaseJudge { get; set; }
        public string? FilingType { get; set; }
        public string? FilingAttorney { get; set; }
        public string? FilingCode { get; set; }
        public string? OrganizationIdentificationID { get; set; }
        public string? CaseCategoryCode { get; set; }
        public string? CaseTypeCode { get; set; }
        public string? DocumentDescriptionText { get; set; }
        public string? DocumentFileControlID { get; set; }
        public string? DocumentFiledDate { get; set; }
        public string? DocumentReceivedDate { get; set; }
        public string? DocumentSubmitterName { get; set; }
        public string? DocumentSubmitterID { get; set; }
        public string? CaseTrackingID { get; set; }
        public string? FilingStatusCode { get; set; }
        public string? StatusDescriptionText { get; set; }
        public string? ENVELOPEID { get; set; }
        public string? FILINGID { get; set; }
    }
}
