using System;

namespace PBL3.Entities
{
    public class LoanAccount : BankAccount
    {
        public double LoanLimit { get; set; } = 30000;
        public double LatePaymentInterestRate { get; set; } = 0.1;
        public double CurrentLoan { get; set; } = 0;

        public LoanAccount() : base()
        {}
        public override void DisplayInfo()
        {
            Console.WriteLine($"LoanAccount - ID: {AccountId}, Balance: {Balance}, Current Loan: {CurrentLoan}, Created Date: {CreatedDate}");
        }

        public override string GetData()
        {
            return $"LoanAccount|{AccountId}|{Balance}|{CurrentLoan}|{CreatedDate}";
        }

        public override string GetAccountType()
        {
            return "LoanAccount";
        }

        public override bool Withdraw(double amount)
        {
            if (Balance + LoanLimit - CurrentLoan >= amount)
            {
                Balance -= amount;
                CurrentLoan += amount;
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
            if (CurrentLoan > 0)
            {
                double interest = CurrentLoan * LatePaymentInterestRate;
                CurrentLoan += interest;
            }
        }
    }
}

