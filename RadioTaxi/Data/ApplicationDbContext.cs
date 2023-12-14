using RadioTaxi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace RadioTaxi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<CategoryPackage> CategoryPackage { get; set; }

        public DbSet<Package> Package { get; set; }

        public DbSet<Company> Company { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
        public DbSet<Advertise> Advertise { get; set; }
        public DbSet<FeedBack> FeedBack { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<Renter> Renter { get; set; }





    }
}
