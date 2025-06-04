namespace PBL3.Models
{
    public class BankAccountViewModel
    {
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsFrozen { get; set; }
        public string AccountType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
