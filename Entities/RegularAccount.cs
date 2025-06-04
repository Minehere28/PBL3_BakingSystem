namespace PBL3.Entities
{
    public class RegularAccount : BankAccount
    {
        public double MonthlyFee { get; set; } = 5;
        public double TransactionFee { get; set; } = 1;

        public RegularAccount() : base(){        }
        public override void DisplayInfo()
        {
            Console.WriteLine($"RegularAccount - ID: {AccountId}, Balance: {Balance}, Created Date: {CreatedDate}");
        }

        public override string GetData()
        {
            return $"RegularAccount|{AccountId}|{Balance}|{CreatedDate}";
        }

        public override string GetAccountType()
        {
            return "RegularAccount";
        }

        public override bool Withdraw(double amount)
        {
            double totalAmount = amount + TransactionFee;
            if (Balance >= totalAmount)
            {
                Balance -= totalAmount;
                return true;
            }
            return false;
        }

        public override bool Transfer(double amount, BankAccount bankAcc)
        {
            if (Withdraw(amount))
        {     
                bankAcc.ReceiveTransfer(amount);
                return true;
            }
            return false;
        }

        public override void MonthlyUpdate()
        {
            if (Balance >= MonthlyFee)
            {
                Balance -= MonthlyFee;
            }
        }
    }
}
