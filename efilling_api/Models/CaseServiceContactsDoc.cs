using System;
using System.Collections.Generic;

namespace efilling_api.Models
{
    public partial class CaseServiceContactsDoc
    {
        public int ServiceContactId { get; set; }
        public int? CaseDocId { get; set; }
        public string? Status { get; set; }
        public DateTime? OpenedAt { get; set; }

        public virtual CaseDocument? CaseDoc { get; set; }
        public virtual CaseServiceContact? ServiceContact { get; set; }
    }
}
