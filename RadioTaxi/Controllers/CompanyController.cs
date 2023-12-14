using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Models.CheckoutVM;
using RadioTaxi.Models.CompanyVM;
using RadioTaxi.Models.DriversVM;
using RadioTaxi.Models.ProfileVM;
using RadioTaxi.Models.ViewMainVM;
using RadioTaxi.Services;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace RadioTaxi.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;



        public CompanyController(ILogger<CompanyController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICommon common, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _iCommon = common;
            _configuration = configuration;
        }

        [HttpGet("/company")]
        public IActionResult Index()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            var listCompany = _context.Company.Include(x => x.ApplicationUserMain)
				.Where(x => x.Status == true && x.Payment == true)
				.Take(20)
                .OrderByDescending(x => x.CreateDate).ToList();
			
			var result = new ViewMainCRUD
			{
				CompanyList = listCompany
			};

			return View(result);

		}
        [HttpGet("/company/profile")]
        public async Task<IActionResult> GetProfile()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            ViewBag.active = "company";
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var company = _context.Company
                     .Include(x => x.ApplicationUserMain)
                     .Include(x => x.PackageMain)
                         .ThenInclude(p => p.Categories)
                     .FirstOrDefault(x => x.UserId == user.Id);
            if (company == null)
            {
                return NotFound();

            }
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

        [HttpGet("/company/join")]
        public async Task<IActionResult> Join()
        {
            var package = _context.Package.Include(x => x.Categories).ToList();

            // Kiểm tra người dùng đã xác thực hay chưa trước khi sử dụng thông tin của họ
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

        //[HttpGet]
        //public async Task<IActionResult> PaypalCallback(string paymentId, string token, string PayerID)
        //{
        //    try
        //    {
        //        var orderId = TempData["OrderId"] as int?;

        //        var executedPayment = await _iCommon.PaypalServices.CapturePayment(paymentId, PayerID);
        //        Orders find = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
        //        if (find != null)
        //        {

        //            find.Order_Status = true;
        //            _context.Orders.Update(find);
        //            await _context.SaveChangesAsync();
        //        }
        //        return Redirect("/success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Redirect("/false");
        //    }
        //}
        [HttpPost("/company/add")]
        public async Task<IActionResult> AddItemsCompany(CompanyCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                    if (userCheck != null)
                    {
                        var newRole = "Company";
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
                            model.UserId = userCheck.Id;
                            model.IDCompany = Guid.NewGuid().ToString();
                            model.CreateDate = DateTime.Now;
                            model.Status = false;
                            model.Payment = false;
                            model.MemberShipType = "Company";

                            _context.Company.Add(model);
                            await _context.SaveChangesAsync();
                            TempData["OrderId"] = model.IDCompany;
                            TempData["Key"] = "Company";
                            string returnUrl = _configuration["PaypalSettings:returnUrl"];
                            string cancelUrl = _configuration["PaypalSettings:cancelUrl"];
                            Package find = _context.Package.Include(x => x.Categories).Where(x => x.ID == model.PackageId).FirstOrDefault();
                            if (find != null)
                            {
                                CheckoutCRUD check = new CheckoutCRUD();
                                check.Price = find.Price;
                                check.NamePackage = find.Name;
                                check.IDkey = model.IDCompany;
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
                                    return Redirect("/company/join");


                                }
                            }

                            TempData["error"] = "Bạn đã có vai trò rồi";
                            return Redirect("/company/join");



                        }


                    }
                }
                else
                {
                    var existingUser = await _userManager.FindByNameAsync(model.Email);
                    if (existingUser != null)
                    {
                        TempData["error"] = " The account already exists on the system";
                        return Redirect("/company/join");


                    }
                    if (model.ConfirmPassword != model.PasswordHash)
                    {
                        TempData["error"] = "Passwords are not the same";

                        return Redirect("/company/join");


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
                    var result = await _userManager.CreateAsync(user, model.PasswordHash);
                    if (result.Succeeded)
                    {

                        await _userManager.AddToRoleAsync(user, "Company");

                        model.UserId = user.Id;
                        model.IDCompany = Guid.NewGuid().ToString();
                        model.CreateDate = DateTime.Now;
                        model.Status = false;
                        model.Payment = false;
                        model.MemberShipType = "Company";

                        _context.Company.Add(model);
                        await _context.SaveChangesAsync();

                        var resultLogin = await _signInManager.PasswordSignInAsync(user.UserName, model.PasswordHash, model.RememberMe, lockoutOnFailure: false);
                        if (resultLogin.Succeeded)
                        {


                            var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, "Company"),
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

                            TempData["OrderId"] = model.IDCompany;
                            TempData["Key"] = "Company";
                            string returnUrl = _configuration["PaypalSettings:returnUrl"];
                            string cancelUrl = _configuration["PaypalSettings:cancelUrl"];
                            Package find = _context.Package.Include(x => x.Categories).Where(x => x.ID == model.PackageId).FirstOrDefault();
                            if (find != null)
                            {
                                CheckoutCRUD check = new CheckoutCRUD();
                                check.Price = find.Price;
                                check.NamePackage = find.Name;
                                check.IDkey = model.IDCompany;
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
                                    return Redirect("/company/join");



                                }
                            }
                            TempData["error"] = " Transaction error";

                            return Redirect("/company/join");


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
                return Redirect("/company/join");




            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect("/company/join");


            }
        }
        [HttpPost("/company/payads")]
        public async Task<IActionResult> PayAds(ProfileCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (userCheck != null)
                    {
                        var driver = await _context.Company.Where(x => x.UserId == userCheck.Id).FirstOrDefaultAsync();
                        if (driver != null)
                        {
                            Advertise ads = new Advertise();
                            ads.CompanyName = driver.ContactPerson;
                            ads.Designation = driver.Designation;
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
                            TempData["IDCompany"] = driver.ID;
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
                TempData["error"] = "Not logged in yet";
                return Redirect("/driver/profile");

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect("/driver/profile");
            }
        }
        [HttpPost("/company/update")]
        public async Task<IActionResult> UpdateDrivers(CompanyCRUD model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {

                    var existingProduct = await _context.Company.FirstOrDefaultAsync(x => x.ID == model.ID);
                    if (existingProduct == null)
                    {
                        return Json(new { code = 404, message = "User not found" });

                    }
                    else
                    {

                        existingProduct.ContactPerson = model.ContactPerson;
                        existingProduct.Designation = model.Designation;
                        existingProduct.Address = model.Address;
                        existingProduct.Mobile = model.Mobile;
                        existingProduct.Telephone = model.Telephone;
                        existingProduct.FaxNumber = model.FaxNumber;
                        existingProduct.Email = model.Email;
                        await _context.SaveChangesAsync();
                    }
                    return Json(new { code = 200, message = "Success" });




                }
                return Json(new { code = 404, message = "User not found" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 404, message = ex.Message });
            }
        }

        [HttpGet("/company/details/{id}")]
        public IActionResult Details(int id)
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            var company = _context.Company.Include(x => x.ApplicationUserMain).Where(x => x.ID == id).FirstOrDefault();
            if (company == null)
            {
                return NotFound();
            }
            var participant = _context.Participants
                .Include(x=>x.DriversMain)
                    .ThenInclude(x=>x.ApplicationUserMain)
                .Include(x=>x.CompanyMain)
                .Where(x=>x.IDCompany == company.ID)
                .Take(20)
                .OrderByDescending(x=>x.CreateDate)
                .ToList();
            if(participant != null)
            {
                var items = new ViewMainCRUD
                {
                    CompanyMain = company,
                    ParticipantsList = participant
                };
                return View(items);
            }
            else
            {
                var items = new ViewMainCRUD
                {
                    CompanyMain = company,
                    ParticipantsList = null
                };
                return View(items);
            }
           
        }
        [HttpPost("/company/driveradd")]
        public async Task<IActionResult> AddDrivers(Participants model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var currentRole = "Company";
                    var userRole = "User";
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    if (!userRoles.Contains(currentRole) && !userRoles.Contains(userRole))
                    {
                        var driver = _context.Drivers.FirstOrDefault(x => x.UserId == userCheck.Id);
                        if (driver != null)
                        {
                            var checkDriver = _context.Participants.Where(x => x.IDDriver == driver.ID).FirstOrDefault();
                            if (checkDriver != null)
                            {
                                return Json(new { code = 404, message = "You already belong to this company" });

                            }
                            else
                            {
                                model.IDDriver = driver.ID;
                                model.CreateDate = DateTime.Now;
                                model.Status = false;
                                _context.Participants.Add(model);
                                await _context.SaveChangesAsync();
                                return Json(new { code = 200, message = "The request was successful, we are reviewing it" });
                            }

                        }

                        return Json(new { code = 404, message = "Driver not found" });

                    }
                    return Json(new { code = 404, message = "You are a company" });

                }
                return Json(new { code = 404, message = "Driver not found" });

            }
            return Json(new { code = 404, message = "You are not logged in" });

        }
        [HttpGet("/company/user")]
        public async Task<IActionResult> DriverRent()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            ViewBag.active = "rent";
            if (User.Identity.IsAuthenticated)
            {
                var currentRole = "Company";

                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    var company = _context.Company.Include(x => x.ApplicationUserMain).FirstOrDefault(x => x.UserId == userCheck.Id);
                    if (company != null)
                    {
                        if (userRoles.Contains(currentRole))
                        {
                            var participants = _context.Participants
                                .Include(x => x.CompanyMain)
                                    .ThenInclude(x=>x.ApplicationUserMain)
                                .Include(x => x.DriversMain)
                                    .ThenInclude(x=>x.ApplicationUserMain)
                                .Where(x => x.IDCompany == company.ID).ToList();
                            if(participants != null)
                            {
                                var items = new ViewMainCRUD
                                {
                                    ParticipantsList = participants,
                                    CompanyMain = company,
                                };
                                return View(items);
                            }

                        }
                        return NotFound();

                    }

                    var itemsNull = new ViewMainCRUD
                    {
                        ParticipantsList = null,
                        CompanyMain = company,

                    };
                    return View(itemsNull);


                }
            }
            return NotFound();
        }

        [HttpPost("/company/change")]
        public async Task<IActionResult> ChangeActive(int id)
        {
            if (User.Identity.IsAuthenticated)
            { 
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userCheck != null)
                {
                    var currentRole = "Company";
                    var userRoles = await _userManager.GetRolesAsync(userCheck);
                    if (userRoles.Contains(currentRole))
                    {
                        var pati = _context.Participants.FirstOrDefault(x => x.ID == id);
                        if (pati != null)
                        {
                            pati.Status = true;
                            _context.Participants.Update(pati);
                            await _context.SaveChangesAsync();
                            return Json(new { code = 200, message = "Success" });
                        }
                    }
                    return Json(new { code = 404, message = "You do not have this authority" });

                }
                return Json(new { code = 404, message = "User not found" });


            }
            return Json(new { code = 404, message = "You are not logged in" });

        }
        [HttpGet("/search/company")]
        public IActionResult GetSearch(string search)
        {

            var products = (from i in _context.Company
                           .Where(i => i.Payment && i.Status && (i.Address.Contains(search) || i.ContactPerson.Contains(search) || i.CompanyName.Contains(search)))
                            select new Company
                            {
                                ID = i.ID,
                                ContactPerson = i.ContactPerson,
								CompanyName = i.CompanyName,
                                Designation = i.Designation,
                                Address = i.Address

                            }).ToList();

            return Ok(products);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}