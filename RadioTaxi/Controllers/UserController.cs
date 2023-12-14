using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Models.AccountVM;
using RadioTaxi.Models.ProfileVM;
using RadioTaxi.Models.ViewMainVM;
using RadioTaxi.Services;
using System.Diagnostics;

namespace RadioTaxi.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _iCommon;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICommon icommon)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _iCommon = icommon;
        }

        [HttpGet("/profile")]
		public async Task<IActionResult> Index()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return NotFound();

            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var listRoles = new List<string>
{
    "Company",
    "Advertise",
    "User",
    "Driver"
};
            var currentRole = userRoles.FirstOrDefault(role => listRoles.Contains(role));
            ViewBag.roles = currentRole;
            
            return View(user);
        }

        [HttpGet("/User/user")]
        public async Task<IActionResult> UserApply()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            ViewBag.active = "rent";
            if (User.Identity.IsAuthenticated)
            {
                var currentRole = "User";

                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    
                        if (userRoles.Contains(currentRole))
                        {
                            var rent = _context.Renter
                                .Include(x => x.ApplicationUserMain)
                                .Include(x => x.DriversMain)
                                    .ThenInclude(x => x.ApplicationUserMain)
                                .Where(x => x.IDUser == userCheck.Id).ToList();
                            if (rent != null)
                            {
                                var items = new ViewMainCRUD
                                {
                                    RenterList = rent,
                                    ApplicationUserMain = userCheck
                                };
                                return View(items);
                            }

                        }
                    var itemsNull = new ViewMainCRUD
                    {
                        RenterList = null,
                        ApplicationUserMain = userCheck

                    };
                    return View(itemsNull);


                }
            }
            return NotFound();
        }
        [HttpPost("/change/img")]
        public async Task<IActionResult> ChangeImg(ProfileCRUD model)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (userCheck != null)
                    {
                        if (model.PrPath != null)
                        {
                            var PrPath = await _iCommon.UploadedFile(model.PrPath);
                            userCheck.AvatartPath = "/upload/" + PrPath;
                        }
                        else
                        {
                            userCheck.AvatartPath = "/upload/avatar/blank_avatar.png";
                        }
                        _context.Update(userCheck);
                        await _context.SaveChangesAsync();
                        return Json(new { code = 200, message = "Upload success" });
                    }

                }
                catch (Exception ex)
                {
                    return Json(new { code = 404, message = ex.Message });

                }
            }
            else
            {
                return Json(new { code = 404, message = "User not found" });

            }
            return Json(new { code = 404, message = "User not found" });


        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}