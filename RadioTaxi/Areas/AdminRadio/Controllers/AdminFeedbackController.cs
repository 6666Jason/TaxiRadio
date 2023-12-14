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
    public class AdminFeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminFeedbackController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet("/AdminRadio/AdminFeedback")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/AdminRadio/AdminFeedback/GetAll")]
        public IActionResult GetAll()
        {
            var pr = _context.FeedBack.Include(x=>x.ApplicationUserMain).OrderByDescending(x => x.ID).ToList(); 
            return Ok(pr);
        }
        
        [HttpGet("/AdminRadio/AdminFeedback/GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                FeedBack vm = new FeedBack();
                if (id > 0)
                {
                    try
                    {
                        vm = _context.FeedBack.FirstOrDefault(x => x.ID == id);

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
       
        [HttpPost("/AdminRadio/AdminFeedback/Delete")]
        public async Task<IActionResult> Delete(FeedBack model)
        {
            try
            {
                var existingProduct = await _context.FeedBack.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                _context.FeedBack.Remove(existingProduct);
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
