using System;
using System.Collections.Generic;

namespace efilling_api.Models
{
    public partial class UserPreference
    {
        public int UserId { get; set; }
        public int? DefaultCaseDesc { get; set; }
        public int? DefaultCourtSystem { get; set; }
        public int? DefaultCourt { get; set; }
        public int? DefaultPlaintiff { get; set; }
        public int? DefaultFilingList { get; set; }
        public int? DefaultScreen { get; set; }
        public int? DefaultServiceContactSelection { get; set; }
        public string? FilingRefreshRate { get; set; }
        public int? DefaultTimzone { get; set; }
        public bool? FilingStatusNotif { get; set; }
        public bool? FileStamped { get; set; }
        public bool? DetailedFilingReceiptAttached { get; set; }
        public bool? FilingStatementAttached { get; set; }
        public string? ConvertToTextPdf { get; set; }
        public bool? AutoCalFees { get; set; }
        public bool? FilingAccepted { get; set; }
        public bool? FilingRejected { get; set; }
        public bool? FilingSubmitted { get; set; }
        public bool? ServiceUndeliverable { get; set; }
        public bool? FilingSubmissionFailed { get; set; }
        public bool? FilingReciepted { get; set; }

        public virtual UserParty? DefaultPlaintiffNavigation { get; set; }
        public virtual UserServiceContact? DefaultServiceContactSelectionNavigation { get; set; }
        public virtual User? User { get; set; }
    }
}
