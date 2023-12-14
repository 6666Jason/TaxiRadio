using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioTaxi.Models
{
    public class FeedBack
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string IDUser { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("IDUser")]
        public virtual ApplicationUser ApplicationUserMain { get; set; }


    }
}