using System;
using System.Collections.Generic;

namespace efilling_api.Models
{
    public partial class AdminSupportStaffPermission
    {
        public int? UserId { get; set; }
        public int? AttorneyId { get; set; }
        public bool? IsAuthorized { get; set; }

        public virtual UserAttorney? Attorney { get; set; }
        public virtual User? User { get; set; }
    }
}
