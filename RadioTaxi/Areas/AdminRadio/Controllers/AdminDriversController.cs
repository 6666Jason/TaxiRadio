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
    public class AdminDriversController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminDriversController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("/AdminRadio/AdminDrivers")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminDrivers/GetAll")]
        public IActionResult GetAll()
        {
            var pr = _context.Drivers.Include(x=>x.PackageMain).Include(x=>x.ApplicationUserMain).OrderByDescending(x => x.ID).ToList(); 
            return Ok(pr);
        }
        [HttpPost("/AdminRadio/AdminDrivers/Add")]
        public async Task<IActionResult> Add(Drivers model)
        {
            try
            {
                model.CreateDate = DateTime.Now;
                model.IDDriver = Guid.NewGuid().ToString();
                _context.Drivers.Add(model);
                await _context.SaveChangesAsync();
                return Ok("succes");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("/AdminRadio/AdminDrivers/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Drivers vm = new Drivers();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.Drivers.FirstOrDefault(x => x.ID == id);

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
        [HttpPost("/AdminRadio/AdminDrivers/Update")]
        public async Task<IActionResult> Update(Drivers model)
        {
            try
            {
                var existingProduct = await _context.Drivers.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                else
                {

                    existingProduct.IDDriver = model.IDDriver;
                    existingProduct.ContactPerson = model.ContactPerson;
                    existingProduct.Address = model.Address;
                    existingProduct.City = model.City;
                    existingProduct.Mobile = model.Mobile;
                    existingProduct.Telephone = model.Telephone;
                    existingProduct.Experience = model.Experience;
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
        [HttpPost("/AdminRadio/AdminDrivers/Delete")]
        public async Task<IActionResult> Delete(Drivers model)
        {
            try
            {
                var existingProduct = await _context.Drivers.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.Drivers.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("/AdminRadio/AdminDrivers/HandleTransaction")]
        public async Task<IActionResult> HandleTransaction(Drivers model)
        {

            try
            {

                var existingProduct = await _context.Drivers.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return BadRequest();

                }
                else
                {
                    existingProduct.Status = true;
                    await _context.SaveChangesAsync();
                }
                return Ok(existingProduct);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("/AdminRadio/AdminDrivers/HandlePayment")]
        public async Task<IActionResult> HandlePayment(Drivers model)
        {

            try
            {

                var existingProduct = await _context.Drivers.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return BadRequest();

                }
                else
                {
                    existingProduct.Payment = true;
                    await _context.SaveChangesAsync();
                }
                return Ok(existingProduct);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
