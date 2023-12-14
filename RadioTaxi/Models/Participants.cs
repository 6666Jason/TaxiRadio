using System.ComponentModel.DataAnnotations.Schema;

namespace RadioTaxi.Models
{
    public class Participants
    {
        public int ID { get; set; }
        public int? IDCompany { get; set; }
        public int? IDDriver { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("IDCompany")]
        public virtual Company? CompanyMain { get; set; }

        [ForeignKey("IDDriver")]
        public virtual Drivers? DriversMain { get; set; }
    }
}
