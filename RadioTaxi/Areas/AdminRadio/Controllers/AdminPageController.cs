using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Services;
using System.Data;

namespace RadioTaxi.Areas.AdminRadio.Controllers
{
    [Area("AdminRadio")]
    [Authorize(Roles = "Admin")]
    public class AdminPageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminPageController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [Authorize]
        //[Authorize(AuthenticationSchemes = "MyCookieAuthenticationScheme")]
        [HttpGet("/AdminRadio/AdminPage")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminPage/GetAllPackage")]
        public IActionResult GetAllPackage()
        {
            var pr = (from c in _context.Package
                      join d in _context.CategoryPackage on c.CategoryID equals d.ID
                      select new
                      {
                          ID = c.ID,
                          Name = c.Name,
                          Price = c.Price,
                          CategoryID = c.CategoryID,
                          NameCategory = d.Name
                      }).OrderByDescending(x => x.ID).ToList();
            return Ok(pr);
        }
        [HttpPost("/AdminRadio/AdminPage/Add")]
        public async Task<IActionResult> Add(Package model)
        {
            try
            {
                _context.Package.Add(model);
                await _context.SaveChangesAsync();
                return Ok("succes");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("/AdminRadio/AdminPage/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Package vm = new Package();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.Package.FirstOrDefault(x => x.ID == id);

                        if (vm == null)
                        {
                            return BadRequest("Không tìm thấy đối tượng với ID tương ứng");
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                }
                else
                {
                    return NotFound();
                }

                return Ok(vm);
            }
            catch (Exception ex)

            {
                return StatusCode(500, ex.Message);

            }
        }
        [HttpPost("/AdminRadio/AdminPage/UpdatePackage")]
        public async Task<IActionResult> UpdatePackage(Package model)
        {
            try
            {
                var existingProduct = await _context.Package.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                else
                {

                    existingProduct.Name = model.Name;
                    existingProduct.Price = model.Price;
                    existingProduct.CategoryID = model.CategoryID;
                    await _context.SaveChangesAsync();
                }

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("/AdminRadio/AdminPage/DeletePackage")]
        public async Task<IActionResult> DeletePackage(Package model)
        {
            try
            {
                var existingProduct = await _context.Package.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.Package.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [AllowAnonymous]
        [HttpPost("/AdminRadio/AdminPage/UploadLocalMain")]
        public IActionResult UploadLocalMain(List<IFormFile> files, [FromServices] IUrlHelperFactory urlHelperFactory)
        {
            var filePaths = new List<string>();

            foreach (IFormFile photo in Request.Form.Files)
            {
                string sv = Path.Combine(_env.WebRootPath, "upload", photo.FileName);
                using (var stream = new FileStream(sv, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                string relativePath = $"~/upload/{photo.FileName}";
                string absolutePath = Url.Content(relativePath);

                filePaths.Add(absolutePath);
            }

            return Json(new { urls = filePaths });
        }
    }
}
