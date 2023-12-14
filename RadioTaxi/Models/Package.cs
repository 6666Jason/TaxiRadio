using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioTaxi.Models
{
    public class Package
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual CategoryPackage Categories { get; set; }



    }
}