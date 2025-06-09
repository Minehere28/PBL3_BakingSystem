using PBL3.Entities;

namespace PBL3.Models
{
    public class AdminHistoryViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Trans> Transactions { get; set; }
        // Các thuộc tính lọc
        public string? SearchBy { get; set; }
        public string? SearchValue { get; set; }

        // Kiểu enum TransactionType giống hệt Trans.Type
        public TransactionType? TransactionType { get; set; }
    }
}
