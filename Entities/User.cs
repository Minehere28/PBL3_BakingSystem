using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Entities
{
    public class User
    {
        private static PasswordHasher<User> hasher = new PasswordHasher<User>();

        [Key]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "SĐT phải có đúng 10 chữ số")]
        public string Sdt { get; set; }//
        [Required]
        public string Hoten { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        [StringLength(20)]
        public string Role { get; set; } 
        public DateTime NS { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public User()
        {
            BankAccounts = new HashSet<BankAccount>();
        }

        // Băm mật khẩu và lưu
        public void SetPassword(string rawPassword)
        {
            PasswordHash = hasher.HashPassword(this, rawPassword);
        }

        // Kiểm tra mật khẩu nhập vào
        public bool VerifyPassword(string inputPassword)
        {
            var result = hasher.VerifyHashedPassword(this, PasswordHash, inputPassword);
            return result == PasswordVerificationResult.Success;
        }

        //public abstract void DisplayInfo();
        //public abstract string GetData();
    }
}
