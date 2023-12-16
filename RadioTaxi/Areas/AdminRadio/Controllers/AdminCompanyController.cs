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
    public class AdminCompany : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminCompany(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("/AdminRadio/AdminCompany")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminCompany/GetAllCompany")]
        public IActionResult GetAllCompany()
        {
            var pr = _context.Company.Include(x=>x.PackageMain).Include(x=>x.ApplicationUserMain).OrderByDescending(x => x.ID).ToList(); 
            return Ok(pr);
        }
        [HttpPost("/AdminRadio/AdminCompany/Add")]
        public async Task<IActionResult> Add(Company model)
        {
            try
            {
                model.CreateDate = DateTime.Now;

                model.IDCompany = Guid.NewGuid().ToString();
                _context.Company.Add(model);
                await _context.SaveChangesAsync();
                return Ok("succes");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("/AdminRadio/AdminCompany/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Company vm = new Company();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.Company.FirstOrDefault(x => x.ID == id);

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
        [HttpPost("/AdminRadio/AdminCompany/Update")]
        public async Task<IActionResult> Update(Company model)
        {
            try
            {
                var existingProduct = await _context.Company.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                else
                {

                    existingProduct.IDCompany = model.IDCompany;
                    existingProduct.ContactPerson = model.ContactPerson;
                    existingProduct.CompanyName = model.CompanyName;
                    existingProduct.Designation = model.Designation;
                    existingProduct.Address = model.Address;
                    existingProduct.Mobile = model.Mobile;
                    existingProduct.Telephone = model.Telephone;
                    existingProduct.FaxNumber = model.FaxNumber;
                    existingProduct.Email = model.Email;
                    existingProduct.MemberShipType = model.MemberShipType;
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
        [HttpPost("/AdminRadio/AdminCompany/Delete")]
        public async Task<IActionResult> Delete(Company model)
        {
            try
            {
                var existingProduct = await _context.Company.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.Company.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("/AdminRadio/AdminCompany/HandleTransaction")]
        public async Task<IActionResult> HandleTransaction(Company model)
        {

            try
            {


                var existingProduct = await _context.Company.FirstOrDefaultAsync(x => x.ID == model.ID);
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
        [HttpPost("/AdminRadio/AdminCompany/HandlePayment")]
        public async Task<IActionResult> HandlePayment(Company model)
        {

            try
            {


                var existingProduct = await _context.Company.FirstOrDefaultAsync(x => x.ID == model.ID);
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
