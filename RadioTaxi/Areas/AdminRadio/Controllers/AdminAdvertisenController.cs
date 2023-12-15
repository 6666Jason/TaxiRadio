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
    public class AdminAdvertisenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminAdvertisenController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("/AdminRadio/AdminAdvertisen")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminAdvertisen/GetAll")]
        public IActionResult GetAll()
        {
            var pr = _context.Advertise.Include(x=>x.PackageMain).OrderByDescending(x => x.ID).ToList(); 
            return Ok(pr);
        }
        [HttpPost("/AdminRadio/AdminAdvertisen/Add")]
        public async Task<IActionResult> Add(Advertise model)
        {
            try
            {
                model.CreateDate = DateTime.Now;
                _context.Advertise.Add(model);
                await _context.SaveChangesAsync();
                return Ok("succes");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("/AdminRadio/AdminAdvertisen/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Advertise vm = new Advertise();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.Advertise.FirstOrDefault(x => x.ID == id);

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
        [HttpPost("/AdminRadio/AdminAdvertisen/Update")]
        public async Task<IActionResult> Update(Advertise model)
        {
            try
            {
                var existingProduct = await _context.Advertise.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                else
                {

                    existingProduct.CompanyName = model.CompanyName;
                    existingProduct.Designation = model.Designation;
                    existingProduct.Address = model.Address;
                    existingProduct.Mobile = model.Mobile;
                    existingProduct.Telephone = model.Telephone;
                    existingProduct.FaxNumber = model.FaxNumber;
                    existingProduct.Email = model.Email;
                    existingProduct.Description = model.Description;
                    existingProduct.PackageId = model.PackageId;
                    existingProduct.CreateDate = DateTime.Now;

                    await _context.SaveChangesAsync();
                }

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("/AdminRadio/AdminAdvertisen/Delete")]
        public async Task<IActionResult> Delete(Advertise model)
        {
            try
            {
                var existingProduct = await _context.Advertise.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.Advertise.Remove(existingProduct);
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
