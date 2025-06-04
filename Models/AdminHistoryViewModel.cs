using PBL3.Entities;

namespace PBL3.Models
{
    public class AdminHistoryViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Trans> Transactions { get; set; }
    }
}
