using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class Court
    {
        public Court()
        {
            CaseTypes = new HashSet<CaseType>();
            Cases = new HashSet<Case>();
            CourtDocuments = new HashSet<CourtDocument>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool HasOptionalServices { get; set; }
        public decimal? TransactionFee { get; set; }
        public int? CountyId { get; set; }

        public virtual County? County { get; set; }
        public virtual ICollection<CaseType> CaseTypes { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<CourtDocument> CourtDocuments { get; set; }
    }
}
