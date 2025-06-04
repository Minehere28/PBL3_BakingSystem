using System.ComponentModel.DataAnnotations;

public class TransferViewModel
{
    public int FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public double balance { get; set; } // Số dư tài khoản người gửi

    [Range(1000, double.MaxValue, ErrorMessage = "Số tiền tối thiểu là 1000đ.")]
    public double Amount { get; set; }
    public string? description { get; set; } // Mô tả giao dịch

    public string AccountUserName { get; set; } // Tên người dùng hiển thị
}
