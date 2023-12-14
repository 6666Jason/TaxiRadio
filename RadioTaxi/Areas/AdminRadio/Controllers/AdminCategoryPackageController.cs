using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Services;
using System.Data;

namespace RadioTaxi.Areas.AdminRadio.Controllers
{
    [Area("AdminRadio")]
    [Authorize(Roles = "Admin")]
    public class AdminCategoryPackageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminCategoryPackageController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("/AdminRadio/AdminCategoryPackage")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminCategoryPackage/GetAll")]
        public IActionResult GetAll()
        {
            var pr = _context.CategoryPackage.OrderByDescending(x => x.ID).ToList(); 
            return Ok(pr);
        }
        [HttpPost("/AdminRadio/AdminCategoryPackage/Add")]
        public async Task<IActionResult> Add(CategoryPackage model)
        {
            try
            {
                _context.CategoryPackage.Add(model);
                await _context.SaveChangesAsync();
                return Ok("succes");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("/AdminRadio/AdminCategoryPackage/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                CategoryPackage vm = new CategoryPackage();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.CategoryPackage.FirstOrDefault(x => x.ID == id);

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
        [HttpPost("/AdminRadio/AdminCategoryPackage/Update")]
        public async Task<IActionResult> Update(CategoryPackage model)
        {
            try
            {
                var existingProduct = await _context.CategoryPackage.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                else
                {

                    existingProduct.Name = model.Name;
                    existingProduct.DateSet = model.DateSet;
                    await _context.SaveChangesAsync();
                }

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("/AdminRadio/AdminCategoryPackage/Delete")]
        public async Task<IActionResult> Delete(CategoryPackage model)
        {
            try
            {
                var existingProduct = await _context.CategoryPackage.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.CategoryPackage.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
