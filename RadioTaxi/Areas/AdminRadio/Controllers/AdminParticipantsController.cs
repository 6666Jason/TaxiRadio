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
    public class AdminParticipantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminParticipantsController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("/AdminRadio/AdminParticipants")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminParticipants/GetAll")]
        public IActionResult GetAll()
        {
            var pr = _context.Participants.Include(x=>x.CompanyMain).Include(X=>X.DriversMain).OrderByDescending(x => x.ID).ToList(); 
            return Ok(pr);
        }
        
        [HttpGet("/AdminRadio/AdminParticipants/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Participants vm = new Participants();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.Participants.FirstOrDefault(x => x.ID == id);

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
       
        [HttpPost("/AdminRadio/AdminParticipants/Delete")]
        public async Task<IActionResult> Delete(Participants model)
        {
            try
            {
                var existingProduct = await _context.Participants.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.Participants.Remove(existingProduct);
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
