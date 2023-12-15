using Microsoft.AspNetCore.Identity;
using RadioTaxi.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RadioTaxi.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace RadioTaxi.Services
{
    public static class NavigationHelper
    {

        public static async Task<string> GetCompanyLink(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext _context)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return "/company";
            }

            var userCheck = await userManager.FindByNameAsync(httpContext.User.Identity.Name);

            if (userCheck != null)
            {
                var userRoles = await userManager.GetRolesAsync(userCheck);
                if (userRoles.Contains("Company"))
                {
                    var company = await _context.Company.FirstOrDefaultAsync(x => x.UserId == userCheck.Id && x.Payment == true);
                    if (company != null)
                    {
                        return $"/company/details/{company.ID}";
                    }
                }
            }
            return "/company";
        }


        public static async Task<string> GetDriverLink(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext _context)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return "/drivers";
            }

            var userCheck = await userManager.FindByNameAsync(httpContext.User.Identity.Name);

            if (userCheck != null)
            {
                var userRoles = await userManager.GetRolesAsync(userCheck);
                if (userRoles.Contains("Driver"))
                {
                    var driver = await _context.Drivers.FirstOrDefaultAsync(x => x.UserId == userCheck.Id && x.Payment == true);
                    if (driver != null)
                    {
                        return $"/driver/details/{driver.ID}";
                    }
                }
            }
            return "/drivers";
        }

        public static async Task<bool> CheckLogin(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            var userCheck = await userManager.FindByNameAsync(httpContext.User.Identity.Name);
            if (userCheck != null)
            {
                return true;
            }
            return false;
         }



    }
}
