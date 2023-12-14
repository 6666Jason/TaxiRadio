using System.ComponentModel.DataAnnotations;

namespace RadioTaxi.Models.AccountVM
{
    public class RegisterVM
    {
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string OptionRole { get; set; }

       

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Không để trống email")]
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }

        public static implicit operator ApplicationUser(RegisterVM vm)
        {
            return new ApplicationUser
            {
                UserName = vm.UserName,
                IsAcitive = true,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                CreateDate = vm.CreateDate,
                FullName = vm.FullName,
            };
        }
    }
}
