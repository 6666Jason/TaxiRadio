using RadioTaxi.Data;
using RadioTaxi.Models;
using RadioTaxi.Models.AccountVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace RadioTaxi.Areas.AdminRadio.Controllers
{
    [Area("AdminRadio")]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("/Admin/UserManager/ChangePassword")]
        public async Task<IActionResult> ChangePassword(string newPass)
        {
            JsonResultVM json = new JsonResultVM();
            try
            {
                var userName = "supperadmin";
                json.Success = true;
                var user = await _userManager.FindByNameAsync(userName);
                user.PasswordHash = null;
                _context.SaveChanges();
                await _userManager.AddPasswordAsync(user, newPass);
                return Ok(json);
            }
            catch (Exception ex)
            {
                json.Success = false;
                json.Mesaage = null;
                json.Object = ex;
                return Ok(json);
            }
        }

        [HttpGet("/Admin/UserManager/Customer")]
        public IActionResult Customer()
        {
            var data = from us in _context.ApplicationUser
                       select new Profile
                       {
                           Id = us.Id,
                           AvatartPath = us.AvatartPath ?? "",
                           FullName = us.FullName ?? "",
                           IsAcitive = us.IsAcitive,
                           UserName = us.UserName,
                           CreateDate = us.CreateDate,
                           Email = us.Email,
                           PackageId = us.PackageId,
                           PhoneNumber = us.PhoneNumber,
                           Address = us.Address,
                       };
            return View(data.ToList());
        }

        [HttpGet("/Admin/UserManager/GetData")]
        public IActionResult GetData()
        {
            JsonResultVM json = new JsonResultVM();
            try
            {
                json.Success = true;
                json.Mesaage = "";
                var data = from us in _context.ApplicationUser
                           select new
                           {
                               us.FullName,
                               IsActive = us.IsAcitive,
                               us.UserName,
                           };
                json.Object = data.ToList();
                return Ok(json);
            }
            catch (Exception ex)
            {
                json.Mesaage = ex.Message;
                json.Success = false;
                json.Object = null;
                return Ok(json);
            }
        }
        [HttpGet("/Admin/UserManager/ChangeActive/{id}")]
        public async Task<IActionResult> ChangeActive(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                user.IsAcitive = !user.IsAcitive;
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/Admin/UserManager/DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
