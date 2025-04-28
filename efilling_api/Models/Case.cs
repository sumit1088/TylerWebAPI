using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class Case
    {
        public Case()
        {
            CaseDocuments = new HashSet<CaseDocument>();
            CaseParties = new HashSet<CaseParty>();
            CaseServiceContacts = new HashSet<CaseServiceContact>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CourtId { get; set; }
        public int? CaseTypeId { get; set; }
        public int? IncidentZipCourt { get; set; }
        public int? JurisdictionalAmount { get; set; }
        public bool? IsConditionallySealed { get; set; }
        public bool? IsComplexCase { get; set; }
        public bool? IsClassAction { get; set; }
        public bool? IsAsbestos { get; set; }
        public bool? IsCalEnvQualityAct { get; set; }
        public string? CaseTitle { get; set; }
        public bool? IsMonetaryRemedy { get; set; }
        public bool? IsPunitiveRemedy { get; set; }
        public bool? IsDeclaratoryInjunctiveRemedy { get; set; }
        public short? NoOfCausesOfActions { get; set; }
        public decimal? RentPerDayAmount { get; set; }
        public short? StreetNo { get; set; }
        public string? StreetName { get; set; }
        public string? Directional { get; set; }
        //changed from string to int
        public int? Suffix { get; set; }
        public string? UnitNo { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public int? CreatedBy { get; set; }
        public int? UserPaymentAccId { get; set; }
        public int? FilingForAttorneyId { get; set; }
        public decimal? TotalFees { get; set; }
        public int? ClientMatterNo { get; set; }
        public string? CourtesyEmailNotice { get; set; }
        public string? NoteToClerk { get; set; }
        public bool? FilingInfoIsVerified { get; set; }
        public int? Status { get; set; }
        public int? EnvelopeNo { get; set; }
        //new for "Filling status table"
        public DateTime? Modified_at { get; set; }

        public virtual CaseType? CaseType { get; set; }
        public virtual Court? Court { get; set; }
        public virtual User? CreatedByNavigation { get; set; }
        public virtual UserAttorney? FilingForAttorney { get; set; }
        public virtual UserPaymentAccount? UserPaymentAcc { get; set; }
        //public virtual LoginRequest? UserLogin { get; set; } 
        public virtual CaseParty? CaseParty { get; set; }

        public virtual ICollection<CaseDocument> CaseDocuments { get; set; }
        public virtual ICollection<CaseParty> CaseParties { get; set; }
        public virtual ICollection<CaseServiceContact> CaseServiceContacts { get; set; }

    }
}
