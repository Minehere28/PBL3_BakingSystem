using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class UnlockViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã tài khoản")]
        public int AccountId { get; set; }
    }
}
