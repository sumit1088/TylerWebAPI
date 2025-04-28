using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tyler_web_app.Models;

namespace tyler_web_app.Models
{
    public class RootJson
    {
        public IEnumerable<Root> Cases { get; set; }
    }
}