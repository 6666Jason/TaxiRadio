using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Models.ViewMainVM;
using RadioTaxi.Services;
using System.Diagnostics;

namespace RadioTaxi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ICommon common)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _iCommon = common;

        }


        public IActionResult Success()
        {
            ViewBag.user = HttpContext.User.Identity.Name;

            return View();

        }

        public IActionResult False()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            return View();
        }
        public IActionResult Index()
        {
            ViewBag.user = HttpContext.User.Identity.Name;

            var listCompany = _context.Company.Include(x => x.ApplicationUserMain).Where(x => x.Status == true && x.Payment == true).ToList();
            var listDrivers = _context.Drivers.Include(x => x.ApplicationUserMain).Where(x => x.Status == true && x.Payment == true).ToList();
           
            var feedback = _context.FeedBack
                .Include(x=>x.ApplicationUserMain)
                .Take(12)
                .OrderByDescending(x=>x.CreateDate)
                .ToList();
            if (listCompany != null && listDrivers != null)
            {
                if(feedback.Any())
                {
                    var items = new ViewMainCRUD
                    {
                        CompanyList = listCompany,
                        DriversList = listDrivers,
                        FeedBackList = feedback
                    };
                    return View(items);
                }
                else
                {
                    var items = new ViewMainCRUD
                    {
                        CompanyList = listCompany,
                        DriversList = listDrivers,
                    };
                    return View(items);
                }
                

            }
            else
            {
                return View();
            }



        }
        [HttpGet("/feedback")] 
        
        public async Task<IActionResult> Feedback()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
            var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if(userCheck != null)
            {
                var items = new ViewMainCRUD
                {
                    ApplicationUserMain = userCheck,
                };
                return View(items);
            }
            return NotFound();
        } 
        [HttpPost("/feedback/add")] 
        public async Task<IActionResult> FeedbackAdd(FeedBack model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userCheck = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if(userCheck != null)
                {
                    //var userRole = "User";
                    //var userRoles = await _userManager.GetRolesAsync(userCheck);
                    //if (userRoles.Contains(userRole))
                    //{
                        model.IDUser = userCheck.Id;
                        model.CreateDate = DateTime.Now;
                        _context.FeedBack.Add(model);
                        await _context.SaveChangesAsync();
                        return Json(new { code = 200, message = "Yêu cầu thành công" });

                    //}
                    //return Json(new { code = 404, message = "Không có quyền feedback" });

                }

                return Json(new { code = 404, message = "User not found" });

            }

            return Json(new { code = 404, message = "You are not logged in" });

        }
        [HttpGet("/price")] 
        public IActionResult Price()
        {
            ViewBag.user = HttpContext.User.Identity.Name;
			var package = _context.Package.Include(x => x.Categories).ToList();


			var items = new ViewMainCRUD
            {
				PackageMain = package,
			};

            return View(items);
        }
		[HttpGet("/priceMain")]
		[ResponseCache(Duration = 86400)] // Lưu cache trong 1 ngày (86400 giây)
		public IActionResult priceMain()
		{
			ViewBag.user = HttpContext.User.Identity.Name;
			var package = _context.Package.Include(x => x.Categories).ToList();

			var items = new ViewMainCRUD
			{
				PackageMain = package,
			};

			return Ok(items);
		}


		public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("/not-found")]
        public IActionResult NotFound404()
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