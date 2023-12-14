using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Services;
using PayPal.Api;
using MailKit.Search;

namespace WebshopBo.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ILogger<CheckoutController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;



        public double TyGiaUsd = 23300;
        public CheckoutController(ILogger<CheckoutController> logger, ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("/success")]

		public IActionResult Success()
        {
            ViewBag.userName = User.Identity.Name;
            var user = _context.ApplicationUser.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                ViewBag.Name = user.FullName;
            }
            return View();

        }
		[HttpGet("/false")]

		public IActionResult False()
        {
            ViewBag.userName = User.Identity.Name;
            var user = _context.ApplicationUser.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                ViewBag.Name = user.FullName;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PaypalCallback(string paymentId, string token, string PayerID)
        {
            try
            {
                var orderId = TempData["OrderId"]?.ToString();

                var key = TempData["Key"] as string;

                var IDCompany = TempData["IDCompany"] as int? ?? 0;

                var IDDriver = TempData["IDDriver"] as int? ?? 0;

                var executedPayment = await _iCommon.PaypalServices.CapturePayment(paymentId, PayerID);
                string redirectUrl = "/success";
                switch (key)
                {
                    case "Company":
                        var company = await _context.Company.FirstOrDefaultAsync(x => x.IDCompany == orderId);
                        if (company != null)
                        {
                            company.Payment = true;
                            _context.Company.Update(company);
                        }
                        redirectUrl = "/company/profile";
                        break;

                    case "Driver":
                        var driver = await _context.Drivers.FirstOrDefaultAsync(x => x.IDDriver == orderId);
                        if (driver != null)
                        {
                            driver.Payment = true;
                            _context.Drivers.Update(driver);
                        }
                        redirectUrl = "/driver/profile";
                        break;
                    case "Advertise":
                        var advertise = await _context.Advertise.FirstOrDefaultAsync(x => x.UserId == orderId);
                        if (advertise != null)
                        {
                            advertise.Payment = true;
                            _context.Advertise.Update(advertise);
                            if (IDCompany != 0)
                            {
                                try
                                {
                                    var companyCheck = _context.Company.Where(x => x.ID == IDCompany).FirstOrDefault();
                                    companyCheck.Status = true;
                                    _context.Company.Update(companyCheck);
                                    redirectUrl = "/company/profile";
                                    break;


                                }
                                catch (Exception ex)
                                {
                                    TempData.Clear();
                                    break;

                                }
                            }
                            else if (IDDriver != 0)
                            {
                                try
                                {
                                    var driverCheck = _context.Drivers.Where(x => x.ID == IDDriver).FirstOrDefault();
                                    driverCheck.Status = true;
                                    _context.Drivers.Update(driverCheck);
                                    redirectUrl = "/driver/profile";
                                    break;

                                }
                                catch (Exception ex)
                                {
                                    TempData.Clear();
                                    break;

                                }


                            }
                        }
                        break;


                    default:
                        break;
                }
                await _context.SaveChangesAsync();
                TempData.Clear();

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                return Redirect("/false");
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