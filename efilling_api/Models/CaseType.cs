using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CaseType
    {
        public CaseType()
        {
            CaseTypeParties = new HashSet<CaseTypeParty>();
            Cases = new HashSet<Case>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal? ProviderFee { get; set; }
        public int? PrimaryCaseType { get; set; }
        public int? CaseStatus { get; set; }
        public int? CourtId { get; set; }

        public virtual Court? Court { get; set; }
        public virtual ICollection<CaseTypeParty> CaseTypeParties { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }
}
