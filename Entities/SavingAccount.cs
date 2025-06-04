using System;

namespace PBL3.Entities
{
    public class SavingAccount : BankAccount
    {
        public float InterestRate { get; set; } = 0.05f;
        public double MinimumBalance { get; set; } = 100;

        public SavingAccount(): base()
        {   }   
        public override void DisplayInfo()
        {
            Console.WriteLine($"SavingAccount - ID: {AccountId}, Balance: {Balance}, Created Date: {CreatedDate}");
        }

        public override string GetData()
        {
            return $"SavingAccount|{AccountId}|{Balance}|{CreatedDate}";
        }

        public override string GetAccountType()
        {
            return "SavingAccount";
        }

        public override bool Withdraw(double amount)
        {
            if (Balance - amount >= MinimumBalance)
            {
                Balance -= amount;
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
            Balance += Balance * InterestRate;
        }
    }
}
