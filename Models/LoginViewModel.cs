using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class LoginViewModel 
    {
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "SĐT phải có đúng 10 chữ số")]
        //[Display(Name = "SDT")]
        public string Sdt { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Password must be at least 5 characters long"), MinLength(5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // Đổi tên để rõ ràng
    }
}
