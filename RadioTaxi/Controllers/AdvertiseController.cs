using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Models.CheckoutVM;
using RadioTaxi.Models.DriversVM;
using RadioTaxi.Models.ProfileVM;
using RadioTaxi.Services;
using System.Diagnostics;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RadioTaxi.Controllers
{
    public class AdvertiseController : Controller
    {
        private readonly ILogger<AdvertiseController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;


        public AdvertiseController(ILogger<AdvertiseController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, ICommon common)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _iCommon = common;

        }

        [HttpGet("/advertise")]
        public async Task<IActionResult> Index()
        {
            var package = _context.Package.Include(x => x.Categories).Where(x => x.Name == "Advertisement").ToList();

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.user = HttpContext.User.Identity.Name;

                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                bool found = false;
                Advertise checkAds = null;
                foreach (var check in package)
                {
                    checkAds = _context.Advertise.Where(x => x.PackageId == check.ID && x.UserId == user.Id).FirstOrDefault();
                    if (checkAds != null)
                    {
                        found = true;
                        break;

                    }
                }
                if (userRoles.Contains("Company"))
                {
                    var company = _context.Company
                       .Include(x => x.ApplicationUserMain)
                       .Include(x => x.PackageMain)
                           .ThenInclude(p => p.Categories)
                       .FirstOrDefault(x => x.UserId == user.Id);
                    if (company != null)
                    {
                        if (found)
                        {
                            var resultsMain = new ProfileCRUD
                            {
                                CompanyMain = company,
                                PackageList = package,
                                CheckAds = false,
                                AdvertiseMain = checkAds
                            };
                            return View(resultsMain);

                        }
                        else
                        {
                            var resultsMain = new ProfileCRUD
                            {
                                CompanyMain = company,
                                PackageList = package,
                                CheckAds = true,
                            };
                            return View(resultsMain);
                        }
                    }
                    else
                    {
                        var resultsMain = new ProfileCRUD
                        {
                            CompanyMain = company,
                            PackageList = package,
                            CheckAds = true,
                        };
                        return View(resultsMain);

                    }
                }
                else if (userRoles.Contains("Driver"))
                {
                    var drivers = _context.Drivers
                       .Include(x => x.ApplicationUserMain)
                       .Include(x => x.PackageMain)
                           .ThenInclude(p => p.Categories)
                       .FirstOrDefault(x => x.UserId == user.Id);
                    if (drivers != null)
                    {
                        var partici = _context.Participants.FirstOrDefault(x => x.IDDriver == drivers.ID);
                        if(partici != null)
                        {
                            if (found)
                            {
                                var resultsMain = new ProfileCRUD
                                {
                                    DriversMain = drivers,
                                    PackageList = package,
                                    CheckAds = false,
                                    AdvertiseMain = checkAds,
                                    DriverCompany = true

                                };
                                return View(resultsMain);

                            }
                            else
                            {
                                var resultsMain = new ProfileCRUD
                                {
                                    DriversMain = drivers,
                                    PackageList = package,
                                    CheckAds = true,
                                    DriverCompany = true

                                };
                                return View(resultsMain);
                            }
                        }
                        else
                        {
                            var resultsMain = new ProfileCRUD
                            {
                                DriversMain = drivers,
                                PackageList = package,
                                CheckAds = true,
                                DriverCompany = false
                            };
                            return View(resultsMain);
                        }
                       
                    }
                    else
                    {
                        var resultsMain = new ProfileCRUD
                        {
                            DriversMain = null,
                            PackageList = package,
                            CheckAds = true,
                            DriverCompany = false

                        };
                        return View(resultsMain);

                    }

                }
            }
                
            var results = new ProfileCRUD
            {
                DriversMain = null,
                CompanyMain = null,
                PackageList = package,
                AdvertiseMain = null,
                CheckAds = true,
            };
            return View(results);
        }
        [HttpGet("/advertise/profile/{idAdvertise}")]
        public async Task<IActionResult> GetProfile(int idAdvertise)
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            ViewBag.active = "advertise";
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var advertise = _context.Advertise
                     .Include(x => x.ApplicationUserMain)
                     .Include(x => x.PackageMain)
                         .ThenInclude(p => p.Categories)
                     .FirstOrDefault(x => x.UserId == user.Id && x.ID == idAdvertise);
            if (advertise == null)
            {
                return NotFound();

            }
            var results = new ProfileCRUD
            {
                AdvertiseMain = advertise,
            };

            return View(results);
        }
        [HttpGet("/advertise/join")]
        public IActionResult Join()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            return View();
        }
        [HttpPost("/advertise/add")]
        public async Task<IActionResult> AddItemsAdvertise(Advertise model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user == null)
                {
                    return Json(new { code = 404, message = "Không tìm thấy người dùng" });
                }

                var newRole = "Advertise";
                var userRoles = await _userManager.GetRolesAsync(user);
                var originalRole = "User";
                var listRoles = new List<string>
{
    "Company",
    "Advertise",
    "User",
    "Driver"
};
                var currentRole = userRoles.FirstOrDefault(role => listRoles.Contains(role));

                if (userRoles.Contains(originalRole) || userRoles.Contains(newRole))
                {
                    if (currentRole.Contains(originalRole))
                    {
                        await _userManager.RemoveFromRoleAsync(user, currentRole);
                    }
                    await _userManager.RemoveFromRoleAsync(user, originalRole);

                    await _userManager.AddToRoleAsync(user, newRole);
                    model.UserId = user.Id;
                    model.CreateDate = DateTime.Now;
                    _context.Advertise.Add(model);
                    await _context.SaveChangesAsync();

                    return Json(new { code = 200, message = "Thành công" });
                }
                return Json(new { code = 404, message = "Bạn đang là " + currentRole });
            }
            catch (Exception ex)
            {
                return Json(new { code = 404, message = "You are not logged in", details = ex.Message });

            }
        }

        [HttpPost("/advertise/payads")]
        public async Task<IActionResult> PayAds(ProfileCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (userCheck != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(userCheck);
                        Advertise ads = new Advertise();

                        if (userRoles.Contains("Company"))
                        {
                            var company = await _context.Company.Where(x => x.UserId == userCheck.Id).FirstOrDefaultAsync();
                            if(company != null)
                            {
                                ads.CompanyName = company.ContactPerson;
                                ads.Designation = company.Designation;
                                ads.Address = company.Address;
                                ads.Mobile = company.Mobile;
                                ads.Telephone = company.Telephone;
                                ads.FaxNumber = company.Mobile;
                                ads.Email = company.Email;
                                ads.Description = "Company";
                                ads.PackageId = model.PackageID;
                                ads.CreateDate = DateTime.Now;
                                ads.UserId = userCheck.Id;
                                ads.Payment = false;
                                _context.Advertise.Add(ads);
                                await _context.SaveChangesAsync();
                                TempData["IDCompany"] = company.ID;

                            }

                        }
                        else if (userRoles.Contains("Driver"))
                        {
                            var driver = await _context.Drivers.Where(x => x.UserId == userCheck.Id).FirstOrDefaultAsync();
                            if(driver != null)
                            {
                                ads.CompanyName = driver.ContactPerson;
                                ads.Designation = "Driver";
                                ads.Address = driver.Address;
                                ads.Mobile = driver.Mobile;
                                ads.Telephone = driver.Telephone;
                                ads.FaxNumber = driver.Mobile;
                                ads.Email = driver.Email;
                                ads.Description = "Driver";
                                ads.PackageId = model.PackageID;
                                ads.CreateDate = DateTime.Now;
                                ads.UserId = userCheck.Id;
                                ads.Payment = false;
                                _context.Advertise.Add(ads);
                                await _context.SaveChangesAsync();
                                TempData["IDDriver"] = driver.ID;

                            }

                        }
                        else
                        {
                            TempData["error"] = " Lỗi bạn đang là user";
                            return Redirect("/advertise");
                        }
                        TempData["OrderId"] = ads.UserId;
                        TempData["Key"] = "Advertise";
                        string returnUrl = _configuration["PaypalSettings:returnUrl"];
                        string cancelUrl = _configuration["PaypalSettings:cancelUrl"];
                        Package find = _context.Package.Include(x => x.Categories).Where(x => x.ID == model.PackageID).FirstOrDefault();
                        if (find != null)
                        {
                            CheckoutCRUD check = new CheckoutCRUD();
                            check.Price = find.Price;
                            check.NamePackage = find.Name;
                            check.IDkey = ads.CompanyName;
                            check.DateSet = find.Categories.DateSet;

                            var createdPayment = await _iCommon.PaypalServices.CreateOrder(check, returnUrl, cancelUrl);
                            string approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href;
                            if (!string.IsNullOrEmpty(approvalUrl))
                            {

                                return Redirect(approvalUrl);
                            }
                            else
                            {
                                TempData["error"] = " Transaction error";
                                return Redirect("/advertise");

                            }
                        }
                    }
                }
                TempData["error"] = "Chưa đăng nhập";
                return Redirect("/advertise");

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect("/advertise");

            }
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