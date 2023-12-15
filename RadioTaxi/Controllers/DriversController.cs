using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Models.DriversVM;
using RadioTaxi.Models.ProfileVM;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using RadioTaxi.Models.CheckoutVM;
using RadioTaxi.Services;
using RadioTaxi.Models.ViewMainVM;

namespace RadioTaxi.Controllers
{
    public class DriversController : Controller
    {
        private readonly ILogger<DriversController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;



        public DriversController(ILogger<DriversController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ICommon common)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _iCommon = common;


        }

        [HttpGet("/drivers")]
        public async Task<IActionResult> Index()
        {

            ViewBag.user = HttpContext.User.Identity.Name;
            var listDriver = _context.Drivers
                .Include(x => x.ApplicationUserMain)
                .Where(x => x.Status == true && x.Payment == true)
                .Take(20)
                .OrderByDescending(x => x.CreateDate)
                .ToList();
            if (User.Identity.IsAuthenticated)
            {
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var userRoles = await _userManager.GetRolesAsync(userCheck);
                if (userRoles.Contains("Driver"))
                {
                    var driver = _context.Drivers.FirstOrDefault(x => x.UserId == userCheck.Id);
                    if (driver != null)
                    {
                        var partici = _context.Participants.FirstOrDefault(x => x.IDDriver == driver.ID && x.Status == true);
                        if(partici != null)
                        {
                            var listOfCompany = _context.Participants
                                .Include(x=>x.CompanyMain)
                                .Include(x=>x.DriversMain)
                                    .ThenInclude(x=>x.ApplicationUserMain)
                                .Where(x=>x.Status == true && x.IDCompany == partici.IDCompany)
                                .ToList();
                            var results = new ViewMainCRUD
                            {
                                ParticipantsList = listOfCompany,
                                IsDriver = false
                            };
                            return View(results);

                        }
                        else
                        {
                            var results = new ViewMainCRUD
                            {
                                ParticipantsList = null,
                                IsDriver = true

                            };
                            return View(results);
                        }
                    }
                }

            }
            var items = new ViewMainCRUD
            {
                DriversList = listDriver,
                IsDriver = false

            };
            return View(items);

        }
        [HttpGet("/driver/profile")]
        public async Task<IActionResult> GetProfile()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            ViewBag.active = "drivers";
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var drivers = _context.Drivers
                     .Include(x => x.ApplicationUserMain)
                     .Include(x => x.PackageMain)
                         .ThenInclude(p => p.Categories)
                     .FirstOrDefault(x => x.UserId == user.Id);

            if (drivers == null)
            {
                return NotFound();

            }
            var partici = _context.Participants.Include(x=>x.CompanyMain).FirstOrDefault(x => x.IDDriver == drivers.ID && x.Status);

            var package = _context.Package.Include(x => x.Categories).Where(x => x.Name == "Advertisement").ToList();
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
            if (found)
            {
                var resultsMain = new ProfileCRUD
                {
                    DriversMain = drivers,
                    PackageList = package,
                    CheckAds = false,
                    AdvertiseMain = checkAds,
                    ParticipantsMain = partici
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
                    ParticipantsMain = partici

                };
                return View(resultsMain);

            }




        }
        [HttpGet("/drivers/join")]
        public async Task<IActionResult> Join()
        {
            var package = _context.Package.Include(x => x.Categories).ToList();

            if (User.Identity.IsAuthenticated)
            {
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck == null)
                {
                    return View();
                }

                var userRoles = await _userManager.GetRolesAsync(userCheck);
                var listRoles = new List<string>
                {
                    "Company",
                    "Advertise",
                    "User",
                    "Driver"
                };

                var currentRole = userRoles.FirstOrDefault(role => listRoles.Contains(role));
                ViewBag.role = currentRole;
                ViewBag.user = userCheck.UserName;

            }
            else
            {
                ViewBag.price = package;

                return View();

            }
            ViewBag.price = package;

            return View();
        }
        [HttpPost("/drivers/add")]
        public async Task<IActionResult> AddItemsDrivers(DriversCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                    if (userCheck != null)
                    {
                        var newRole = "Driver";
                        var userRoles = await _userManager.GetRolesAsync(userCheck);
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
                                await _userManager.RemoveFromRoleAsync(userCheck, currentRole);
                            }
                            await _userManager.RemoveFromRoleAsync(userCheck, originalRole);

                            await _userManager.AddToRoleAsync(userCheck, newRole);
                            model.IDDriver = Guid.NewGuid().ToString();
                            model.UserId = userCheck.Id;
                            model.CreateDate = DateTime.Now;
                            model.Status = false;
                            model.Payment = false;
                            model.Description = "Description...";
                            model.Experience = "Experience...";
                            _context.Drivers.Add(model);
                            await _context.SaveChangesAsync();

                            TempData["OrderId"] = model.IDDriver;
                            TempData["Key"] = "Driver";
                            string returnUrl = _configuration["PaypalSettings:returnUrl"];
                            string cancelUrl = _configuration["PaypalSettings:cancelUrl"];
                            Package find = _context.Package.Include(x => x.Categories).Where(x => x.ID == model.PackageId).FirstOrDefault();
                            if (find != null)
                            {
                                CheckoutCRUD check = new CheckoutCRUD();
                                check.Price = find.Price;
                                check.NamePackage = find.Name;
                                check.IDkey = model.IDDriver;
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
                                    return Redirect("/drivers/join");


                                }
                            }

                            TempData["error"] = " Transaction error";
                            return Redirect("/drivers/join");

                        }


                    }
                }

                else
                {
                    var existingUser = await _userManager.FindByNameAsync(model.Email);
                    if (existingUser != null)
                    {
                        TempData["error"] = " The account already exists on the system";
                        return Redirect("/drivers/join");
                    }
                    if (model.ConfirmPassword != model.PasswordHash)
                    {
                        TempData["error"] = "Passwords are not the same";

                        return Redirect("/drivers/join");

                    }
                    ApplicationUser user = new ApplicationUser();

                    if (model.Path != null)
                    {
                        var PrPath = await _iCommon.UploadedFile(model.Path);
                        user.AvatartPath = "/upload/" + PrPath;
                    }
                    else
                    {
                        user.AvatartPath = "/upload/avatar/blank_avatar.png";
                    }
                    user.IsAcitive = true;
                    user.PhoneNumber = model.Mobile;
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.FullName = model.ContactPerson;
                    user.Address = model.Address;
                    var result = await _userManager.CreateAsync(user, model.PasswordHash);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Driver");

                        model.IDDriver = Guid.NewGuid().ToString();
                        model.CreateDate = DateTime.Now;
                        model.Status = false;
                        model.Payment = false;
                        model.UserId = user.Id;
                        model.Description = "Description...";
                        model.Experience = "Experience...";

                        _context.Drivers.Add(model);

                        await _context.SaveChangesAsync();
                        var resultLogin = await _signInManager.PasswordSignInAsync(user.UserName, model.PasswordHash, model.RememberMe, lockoutOnFailure: false);
                        if (resultLogin.Succeeded)
                        {
                            var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, "Driver"),
                    };
                            var claimsIdentity = new ClaimsIdentity(
                             claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                            };
                            await HttpContext.SignInAsync(
                               CookieAuthenticationDefaults.AuthenticationScheme,
                               new ClaimsPrincipal(claimsIdentity),
                               authProperties);
                            TempData["OrderId"] = model.IDDriver;
                            TempData["Key"] = "Driver";
                            string returnUrl = _configuration["PaypalSettings:returnUrl"];
                            string cancelUrl = _configuration["PaypalSettings:cancelUrl"];
                            Package find = _context.Package.Include(x => x.Categories).Where(x => x.ID == model.PackageId).FirstOrDefault();
                            if (find != null)
                            {
                                CheckoutCRUD check = new CheckoutCRUD();
                                check.Price = find.Price;
                                check.NamePackage = find.Name;
                                check.IDkey = model.IDDriver;
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
                                    return Redirect("/drivers/join");




                                }
                            }
                            TempData["error"] = " Transaction error";

                            return Redirect("/drivers/join");

                        }
                        else
                        {
							foreach (var error in result.Errors)
							{
								TempData["error"] = error.Description;
								return Redirect("/drivers/join");

							}
						}
						
					}
                }
                TempData["error"] = " Transaction error";

                return Redirect("/drivers/join");


            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect("/drivers/join");

            }
        }
        [HttpPost("/drivers/payads")]
        public async Task<IActionResult> PayAds(ProfileCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (userCheck != null)
                    {
                        var driver = await _context.Drivers.Where(x => x.UserId == userCheck.Id).FirstOrDefaultAsync();
                        if (driver != null)
                        {
                            Advertise ads = new Advertise();
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

                            TempData["OrderId"] = ads.UserId;
                            TempData["Key"] = "Advertise";
                            TempData["IDDriver"] = driver.ID;

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
                                    return Redirect("/driver/profile");

                                }
                            }

                        }

                    }
                }
                TempData["error"] = "Chưa đăng nhập";
                return Redirect("/driver/profile");

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect("/driver/profile");
            }
        }

        [HttpPost("/drivers/update")]
        public async Task<IActionResult> UpdateDrivers(DriversCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {

                    var existingProduct = await _context.Drivers.FirstOrDefaultAsync(x => x.ID == model.ID);
                    if (existingProduct == null)
                    {
                        return Json(new { code = 404, message = "Không tìm thấy người dùng" });

                    }
                    else
                    {

                        existingProduct.ContactPerson = model.ContactPerson;
                        existingProduct.Address = model.Address;
                        existingProduct.City = model.City;
                        existingProduct.Mobile = model.Mobile;
                        existingProduct.Telephone = model.Telephone;
                        existingProduct.Experience = model.Experience;
                        existingProduct.Email = model.Email;
                        existingProduct.Description = model.Description;
                        await _context.SaveChangesAsync();
                    }
                    return Json(new { code = 200, message = "Thành công" });




                }
                return Json(new { code = 404, message = "Không tìm thấy người dùng" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 404, message = ex.Message });
            }
        }
        [HttpGet("/search/driver")]
        public IActionResult GetSearch(string search)
        {

            var products = (from i in _context.Drivers
							.Where(i => i.Payment && i.Status && (i.Address.Contains(search) || i.City.Contains(search)))
                            select new Drivers
                            {
                                ID = i.ID,
                                ContactPerson = i.ContactPerson,
                                City = i.City,
                                Address = i.Address

                            }).ToList();

            return Ok(products);
        }
        [HttpGet("/driver/details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            var driver = _context.Drivers.Include(x => x.ApplicationUserMain).Where(x => x.ID == id).FirstOrDefault();
            if (driver == null)
            {
                return NotFound();
            }
            var partici = _context.Participants
                .Include(x => x.DriversMain)
                    .ThenInclude(x => x.ApplicationUserMain)
                .Include(x => x.CompanyMain)
                .FirstOrDefault(x => x.Status);
            if (partici != null)
            {
                var items = new ViewMainCRUD
                {
                    DriversMain = driver,
                    ParticipantsMain = partici,
                };
                return View(items);
            }
            else
            {
                var items = new ViewMainCRUD
                {
                    DriversMain = driver,
                };
                return View(items);
            }
           
        }
        [HttpPost("/driver/rent")]
        public async Task<IActionResult> AddDrivers(Renter model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var currentRole = "Driver";
                    var userRole = "Company";
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    if (!userRoles.Contains(currentRole) && !userRoles.Contains(userRole))
                    {
                        var checkDriver = _context.Renter.Where(x => x.IDDriver == model.IDDriver && x.IDUser == userCheck.Id && x.Status == false).FirstOrDefault();
                        if (checkDriver != null)
                        {
                            return Json(new { code = 404, message = "Vui lòng chờ xác nhận từ Driver" });

                        }
                        else
                        {
                            model.IDUser = userCheck.Id;
                            model.CreateDate = DateTime.Now;
                            model.Status = false;
                            _context.Renter.Add(model);
                            await _context.SaveChangesAsync();
                            return Json(new { code = 200, message = "Yêu cầu thành công" });
                        }
                    }
                    return Json(new { code = 404, message = "Bạn đang là driver hoặc company" });

                }
                return Json(new { code = 404, message = "User not found" });

            }
            return Json(new { code = 404, message = "You are not logged in" });

        }
        [HttpGet("/driver/user")]
        public async Task<IActionResult> UserRent()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            ViewBag.active = "rent";
            if (User.Identity.IsAuthenticated)
            {
                var currentRole = "Driver";

                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    var driver = _context.Drivers.Include(x => x.ApplicationUserMain).FirstOrDefault(x => x.UserId == userCheck.Id);
                    if (driver != null)
                    {
                        if (userRoles.Contains(currentRole))
                        {
                            var rent = _context.Renter
                                .Include(x => x.ApplicationUserMain)
                                .Include(x => x.DriversMain)
                                    .ThenInclude(x => x.ApplicationUserMain)
                                .Where(x => x.IDDriver == driver.ID).ToList();
                            if (rent != null)
                            {
                                var items = new ViewMainCRUD
                                {
                                    RenterList = rent,
                                    DriversMain = driver
                                };
                                return View(items);
                            }

                        }
                        return NotFound();

                    }

                    var itemsNull = new ViewMainCRUD
                    {
                        RenterList = null,
                        DriversMain = driver

                    };
                    return View(itemsNull);


                }
            }
            return NotFound();
        }
        [HttpPost("/driver/change")]
        public async Task<IActionResult> ChangeActive(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var currentRole = "Driver";
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    if (userRoles.Contains(currentRole))
                    {
                        var rent = _context.Renter.FirstOrDefault(x => x.ID == id);
                        if (rent != null)
                        {
                            rent.Status = true;
                            _context.Renter.Update(rent);
                            await _context.SaveChangesAsync();
                            return Json(new { code = 200, message = "Thành công" });
                        }
                    }
                    return Json(new { code = 404, message = "You do not have this authority" });

                }
                return Json(new { code = 404, message = "User not found" });


            }
            return Json(new { code = 404, message = "You are not logged in" });

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