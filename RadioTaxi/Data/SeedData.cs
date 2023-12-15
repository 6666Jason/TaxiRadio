using RadioTaxi.Models;
using Microsoft.AspNetCore.Identity;

namespace RadioTaxi.Data
{
    public interface IIdentityDataInitializer
    {
        Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager);
    }

    public class IdentityDataInitializer : IIdentityDataInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public IdentityDataInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            // Add roles          
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Company"));
            await roleManager.CreateAsync(new IdentityRole("Advertise"));
            await roleManager.CreateAsync(new IdentityRole("User"));
            await roleManager.CreateAsync(new IdentityRole("Driver"));

            // Add super admin user
            var superAdminEmail = _configuration["SuperAdminDefaultOption:Email"];
            var superAdminUserName = _configuration["SuperAdminDefaultOption:Username"];
            var superAdminPassword = _configuration["SuperAdminDefaultOption:Password"];
            var superAdminUser = new ApplicationUser
            {
                Email = superAdminEmail,
                UserName = superAdminUserName,
                AvatartPath = "/upload/avatar/blank_avatar.png",
                FullName = "superAdminUserName",
                IsAcitive = true,
            };
            
            var result = await userManager.CreateAsync(superAdminUser, superAdminPassword);
        
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdminUser, "Admin");
            }
        }
    }
}
