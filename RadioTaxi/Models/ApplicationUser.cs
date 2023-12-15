using Microsoft.AspNetCore.Identity;

namespace RadioTaxi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatartPath { get; set; }
        public string? FullName { get; set; }
        public string? Address { set; get;}
        public bool IsAcitive { get; set; }
        public int PackageId { get; set; }

       /* public string OTP {  get; set; }*/
        public DateTime CreateDate { get; set; }
        public DateTime CreateDatePackage { get; set; }
        public DateTime EndDatePackage { get; set; }

    }
}
