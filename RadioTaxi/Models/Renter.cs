using System.ComponentModel.DataAnnotations.Schema;

namespace RadioTaxi.Models
{
    public class Renter
    {
        public int ID { get; set; }
        public string? IDUser { get; set; }
        public int? IDDriver { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("IDUser")]
        public virtual ApplicationUser? ApplicationUserMain { get; set; }

        [ForeignKey("IDDriver")]
        public virtual Drivers? DriversMain { get; set; }
    }
}
