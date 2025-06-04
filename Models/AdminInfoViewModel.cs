namespace PBL3.Models
{

    public class AdminInfoViewModel
    {
        public string HoTen { get; set; }
        public string Username { get; set; }
        public DateTime NgaySinh { get; set; }

        // Dành cho cập nhật mật khẩu lấy từ database
        public string MatKhauMoi { get; set; }
        public string XacNhanMatKhauMoi { get; set; }
    }
}