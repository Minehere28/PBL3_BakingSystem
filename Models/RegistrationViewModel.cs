using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class RegistrationViewModel
    {
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "SĐT phải có đúng 10 chữ số")]
        public string Sdt { get; set; }
        [Required(ErrorMessage = "This field is requierd")]
        public string Hoten { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters long"), MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // Đổi tên để rõ ràng
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } // Xác nhận mật khẩu

        [DataType(DataType.DateTime)]
        public DateTime NS { get; set; }
        [Required(ErrorMessage = "This field is requierd")]
        public string AccountType { get; set; } // Loại tài khoản (Vay, Tiet kiem, thong thuong)


    }
}
