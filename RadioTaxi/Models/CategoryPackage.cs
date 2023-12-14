using System.ComponentModel.DataAnnotations;

namespace RadioTaxi.Models
{
    public class CategoryPackage
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int DateSet { get; set; }



    }
}