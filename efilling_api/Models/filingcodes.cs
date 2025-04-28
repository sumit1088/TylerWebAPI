using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class filingcodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? fee { get; set; }
        public string? casecategory { get; set; }
        public string? casetypeid { get; set; }
        public string? filingtype { get; set; }
        public string? iscourtuseonly { get; set; }
        public string? civilclaimamount { get; set; }
        public string? probateestateamount { get; set; }
        public string? amountincontroversy { get; set; }
        public string? useduedate { get; set; }
        public string? isproposedorder { get; set; }
        public string? excludecertificateofservice { get; set; }
        public string? iswaiverrequest { get; set; }
        public string? efspcode { get; set; }
    }
}
