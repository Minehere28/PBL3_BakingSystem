using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class WithdrawViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập ID tài khoản.")]
        [Display(Name = "ID tài khoản")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền cần rút.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải từ 1000 VND trở lên")]
        [Display(Name = "Số tiền rút")]
        public double Amount { get; set; }
    }
}
