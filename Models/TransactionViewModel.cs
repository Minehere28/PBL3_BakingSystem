namespace PBL3.Models
{
    public class TransactionViewModel
    {
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Amount { get; set; }
        public bool IsInflow { get; set; } // true: tiền vào, false: tiền ra
        public string PartyLabel { get; set; } // "Từ" hoặc "Đến"
        public string PartyInfo { get; set; }  // "6789 - Nguyễn Thị Sương Mai"
    }
}
