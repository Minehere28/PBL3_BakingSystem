using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3.Entities
{
    public enum TransactionType
    {
        Deposit,   // Nạp tiền
        Withdrawal, // Rút tiền
        Transfer,   // Chuyển khoản
        Savings,
        Loan
    }
    public class Trans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong TransactionId { get; set; } 
        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now; 

        [Required]
        public TransactionType Type { get; set; } 
        public string? Description { get; set; }
        public double? ReceiverBalanceAfter { get; set; }
        public double? SenderBalanceAfter { get; set; }

        // Khóa ngoại
        [ForeignKey("FromAccount")]
        public int? FromAccountId { get; set; }

        [ForeignKey("ToAccount")]
        public int? ToAccountId { get; set; }

        public double Amount { get; set; }

        // Mối quan hệ với các đối tượng BankAccount
        public virtual BankAccount FromAccount { get; set; }
        public virtual BankAccount ToAccount { get; set; }
        public Trans()
        {

        }
        public Trans(int? fromAccountId, int? toAccountId, BankAccount? fromAccount, BankAccount? toAccount, double amount, TransactionType type, string? description)
        {
            if (amount <= 0)
                throw new ArgumentException("Số tiền phải lớn hơn 0", nameof(amount));
            if (fromAccountId.HasValue && toAccountId.HasValue && fromAccountId == toAccountId)
                throw new ArgumentException("Tài khoản gửi và tài khoản nhận không thể giống nhau", nameof(fromAccountId));

            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            Type = type;
            TransactionDate = DateTime.Now;
            TransactionId = GenerateUniqueTransactionId();
            Description = description;
        }

        // Hàm tạo TransactionId duy nhất
        public static ulong GenerateUniqueTransactionId()
        {
            // Cách tạo ID duy nhất bằng cách kết hợp ticks với một số ngẫu nhiên
            return (ulong)(DateTime.Now.Ticks + new Random().Next(1, 1000)) % ulong.MaxValue;
        }
    }
}
