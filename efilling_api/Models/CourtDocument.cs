using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CourtDocument
    {
        public CourtDocument()
        {
            CaseDocuments = new HashSet<CaseDocument>();
            CourtOptionalServices = new HashSet<CourtOptionalService>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal? DocFee { get; set; }
        public int? DocType { get; set; }
        public int? CourtId { get; set; }
        public decimal? Qty { get; set; }

        public virtual Court? Court { get; set; }
        public virtual ICollection<CaseDocument> CaseDocuments { get; set; }
        public virtual ICollection<CourtOptionalService> CourtOptionalServices { get; set; }
    }
}
