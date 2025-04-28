using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class Lookup
    {
        public Lookup()
        {
            LookupValues = new HashSet<LookupValue>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? LookupName { get; set; }

        public virtual ICollection<LookupValue> LookupValues { get; set; }
    }
}
