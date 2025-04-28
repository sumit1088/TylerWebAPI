using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class LookupValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? LookupValue1 { get; set; }
        //public int? LookupId { get; set; } // added automatically

        public virtual Lookup? Lookup { get; set; }
    }
}
