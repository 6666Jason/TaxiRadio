using RadioTaxi.Data;
using RadioTaxi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RadioTaxi.Services;
using Microsoft.Extensions.Options;
using RadioTaxi.Models.AccountVM;

namespace ShopBanVe.Controllers
{
    public static class CustomClaimTypes
    {
        public const string FullName = "FullName";
    }


    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginVM> _logger;
        private readonly IConfiguration _iConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _icommon;

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, ILogger<LoginVM> logger, IConfiguration iConfiguration, ICommon icommon)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _iConfiguration = iConfiguration;
            _userManager = userManager;
            _icommon = icommon;
        }



        [HttpGet]
        public IActionResult Register() { return View(); }



        [Route("/tai-khoan/quen-mat-khau")]
        [HttpGet]
        public IActionResult QuenMatKhau() { return View(); }


        [Route("/tai-khoan/dang-nhap")]

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);


            if (user != null && !user.IsAcitive)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản bị khoá");
                ViewBag.Er = "Tài khoản bị khoá";
            }
            //if (!_validate.HasRequestValidCaptchaEntry())
            //{
            //    return Json(new { code = 408, message = "Vui lòng nhập mã xác nhận hoặc kiểm tra lại mã xác nhận" });

            //}
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var role = await _userManager.GetRolesAsync(user);
                try
                {

                    var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role.FirstOrDefault()!),
                };

                    // Xây dựng ClaimsIdentity
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Thiết lập các thuộc tính xác thực (nếu có)
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Thiết lập cho phép lưu cookie vĩnh viễn (remember me)
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1), // Thiết lập thời gian hết hạn sau 2 giờ
                    };


                    // Đăng ký phiên đăng nhập hiện tại vào HttpContext
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    if (role.Contains("Admin"))
                    {
                        return Json(new { code = 208, message = "Success", red = "/AdminRadio/AdminPage" });
                    }
                    else
                    {
                        return Json(new { code = 200, message = "Success", section = true });
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }


            }
            else
            {

                ModelState.AddModelError(string.Empty, "Account or password does not exist");
                return Json(new { code = 400, message = "Account or password does not exist" });

            }


        }


        [Route("/tai-khoan/dang-ky-tai-khoan")]

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {

            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                // Username already exists
                ModelState.AddModelError("UserName", "The account already exists on the system");
                return Json(new { code = 400, message = "The account already exists on the system" });

            }
            if (model.ConfirmPassword != model.PasswordHash)
            {
                return Json(new { code = 400, message = "Passwords are not the same" });
            }
            ApplicationUser user = model;
            user.AvatartPath = "/Upload/avatar/blank_avatar.png";
            user.IsAcitive = true;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.CreateDate = DateTime.Now;
            user.FullName = model.FullName;
            user.UserName = model.UserName;
            var result = await _userManager.CreateAsync(user, model.PasswordHash);

            if (result.Succeeded)
            {
                //_icommon.SendEmail(user);
                // Set role member
                await _userManager.AddToRoleAsync(user, "User");
                // Automatically sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);

                return Json(new { code = 200, message = "Success", section = true });
            }
            foreach (var error in result.Errors)
            {

                ModelState.AddModelError("", error.Description);
                return Json(new { code = 400, message = "Password must have at least 1 uppercase letter and special character" });

            }

            return Json(new { code = 400, message = "Check the fields again" });

        }

        [Route("/dang-xuat")]

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
         new AuthenticationProperties { RedirectUri = "/Home/Index" }
          );
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getMomneyTotal()
        {
            string money = "";
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return Ok(money);
        }

        //[HttpGet]
        //public IActionResult GetAllBank()
        //{
        //    var pr = _context.BankConfig.ToList().OrderByDescending(x => x.ID);
        //    return Ok(pr);
        //}
        //[HttpPost]
        //public async Task<IActionResult> UpdateRechaUser(HistoryCRUD model)
        //{
        //    try
        //    {
        //        ViewBag.userName = User.Identity.Name;
        //        var user = _context.ApplicationUser.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
        //        if (user != null)
        //        {
        //            History buy = new History();
        //            buy.isDone = false;
        //            buy.UserId = user.Id;
        //            buy.PriceUser = model.PriceUser;
        //            buy.BankId = model.BankId;
        //            buy.ContentTransit = model.ContentTransit;
        //            buy.CreateDate = DateTime.Now;

        //            await _context.AddAsync(buy);

        //            await _context.SaveChangesAsync();
        //            return new JsonResult(new
        //            {
        //                code = 200,
        //                status = "Successs",
        //                message = "Đã thêm dữ liệu"
        //            });
        //        }
        //        else
        //        {

        //            return new JsonResult(new
        //            {
        //                code = 405,
        //                status = "error",
        //                message = "Bạn chưa đăng nhập"
        //            });
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            code = 400,
        //            status = "error",
        //            message = ex.Message
        //        });
        //    }
        //}
        public static string GenerateRandomString(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = characters[random.Next(characters.Length)];
            }

            return new string(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> CheckUserName(string email)
        //{
        //    var existingUser = await _userManager.FindByNameAsync(email);
        //    if(existingUser != null)
        //    {
        //        string randomString = GenerateRandomString(5);
        //        existingUser.TokenEmail = randomString;

        //        await _userManager.UpdateAsync(existingUser);

        //        _icommon.SendEmailUserMatKhau(existingUser);
        //        return new JsonResult(new
        //        {
        //            code = 200,
        //            status = "Successs",
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            code = 400,
        //            status = "error",
        //            message = "Không tìm thấy email này"
        //        });
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> CheckUserNameOTP(string email, string OTP)
        //{
        //    var existingUser = await _userManager.FindByNameAsync(email);
        //    if(existingUser != null)
        //    {
        //        if(existingUser.TokenEmail != OTP)
        //        {
        //            return new JsonResult(new
        //            {
        //                code = 400,
        //                status = "error",
        //                message = "Mã OTP không đúng"
        //            });
        //        }
        //        return new JsonResult(new
        //        {
        //            code = 200,
        //            status = "Successs",
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            code = 400,
        //            status = "error",
        //            message = "Không tìm thấy email này"
        //        });
        //    }
        //} 

        //[HttpPost]
        //public async Task<IActionResult> ChangePassUserNameOTP(string email, string OTP, string newPass)
        //{
        //    var existingUser = await _userManager.FindByNameAsync(email);
        //    if(existingUser != null)
        //    {
        //        if(existingUser.TokenEmail != OTP)
        //        {
        //            return new JsonResult(new
        //            {
        //                code = 400,
        //                status = "error",
        //                message = "Mã OTP không đúng"
        //            });
        //        }
        //        var removePasswordResult = await _userManager.RemovePasswordAsync(existingUser);
        //        if (!removePasswordResult.Succeeded)
        //        {
        //            return BadRequest();
        //        }
        //        var addPasswordResult = await _userManager.AddPasswordAsync(existingUser, newPass);
        //        if (!addPasswordResult.Succeeded)
        //        {
        //            return BadRequest();
        //        }

        //        return new JsonResult(new
        //        {
        //            code = 200,
        //            status = "success",
        //            message = "Đổi mật khẩu thành công"
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            code = 400,
        //            status = "error",
        //            message = "Không tìm thấy email này"
        //        });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateUser(UpdateProfileVM vm)
        //{
        //    JsonResultVM json = new JsonResultVM();
        //    try
        //    {
        //        var user = await _userManager.FindByNameAsync(vm.userName);
        //        if (vm.AvatarFile != null)
        //        {
        //            user.AvatartPath = await _icommon.UploadAvatar(vm.AvatarFile);
        //        }
        //        if (vm.Email != null)
        //        {
        //            user.Email = vm.Email;
        //            await _userManager.UpdateNormalizedEmailAsync(user);
        //            user.EmailConfirmed = false;
        //        }
        //        user.PhoneNumber = vm.Phone;
        //        user.Address = vm.Address;
        //        user.InvitedCode = vm.InvitedCode;
        //        user.FullName = vm.Name;
        //        await _userManager.UpdateAsync(user);
        //        json.Success = true;
        //        return Ok(json);
        //    }
        //    catch (Exception ex)
        //    {
        //        json.Success = false;
        //        json.Object = ex;
        //        json.Mesaage = ex.Message;
        //        return Ok(json);
        //    }

        //}
        //[HttpGet]
        //public async Task<IActionResult> ConfirmMail(string userName, string mail)
        //{
        //    JsonResultVM json = new JsonResultVM();
        //    try
        //    {
        //        var user = await _userManager.FindByNameAsync(userName);
        //        user.EmailConfirmed = true;
        //        await _userManager.UpdateAsync(user);
        //        json.Success = true;
        //        json.Object = user;
        //        return Redirect(_iConfiguration["domain"] + "/Home/Profile");
        //    }
        //    catch (Exception ex)
        //    {
        //        json.Success = false;
        //        json.Object = ex;
        //        return Redirect(_iConfiguration["domain"] + "/Home/Profile");
        //    }
        //}
        //[HttpGet]
        //public async Task<IActionResult> ConfirmMialByClick(string email)
        //{
        //    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        //    bool check = _icommon.VerifyMail(user.UserName, email);
        //    return Ok(check);
        //}
        //[HttpGet]
        //public async Task<IActionResult> ForGotPass(string email)
        //{
        //    string newPass = "user123456";
        //    var user = await _userManager.FindByEmailAsync(email);
        //    user.PasswordHash = null;
        //    _context.SaveChanges();
        //    await _userManager.AddPasswordAsync(user, newPass);
        //    bool check = _icommon.SendMailChangePassWord(email, newPass);
        //    return Ok(check);
        //}
        //[HttpGet]
        //public async Task<IActionResult> ChangePassWork(string userName, string newPass)
        //{
        //    JsonResultVM json = new JsonResultVM();
        //    try
        //    {
        //        json.Success = true;
        //        var user = await _userManager.FindByNameAsync(userName);
        //        user.PasswordHash = null;
        //        _context.SaveChanges();
        //        await _userManager.AddPasswordAsync(user, newPass);
        //        _icommon.SendMailChangePassWord(user.Email, newPass);
        //        return Ok(json);
        //    }
        //    catch (Exception ex)
        //    {
        //        json.Success = false;
        //        json.Mesaage = null;
        //        json.Object = ex;
        //        return Ok(json);
        //    }
        //}
    }
}
