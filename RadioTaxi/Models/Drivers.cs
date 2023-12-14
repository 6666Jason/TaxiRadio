using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace RadioTaxi.Models
{
    public class Drivers
    {
        [Key]
        public int ID { get; set; }
        public string IDDriver { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public string Mobile { get; set; }
        public string Telephone { get; set; }

        public string? Experience { get; set; }
        public string Email { get; set; }
        public string? Description  { get; set; }
        public int PackageId  { get; set; }
        public string  UserId  { get; set; }
        public bool Status { get; set; }
        public bool Payment { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("PackageId")]
        public virtual Package PackageMain { get; set; }
        
        [ForeignKey("UserId")]
        [Required]
        public virtual ApplicationUser ApplicationUserMain { get; set; }

    }
}