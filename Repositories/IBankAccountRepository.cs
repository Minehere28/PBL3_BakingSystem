﻿using PBL3.Entities;

namespace PBL3.Repositories
{
    public interface IBankAccountRepository
    {
        RegularAccount? GetRegularAccountBySdt(string sdt);
        LoanAccount? GetLoanAccountBySdt(string sdt);
        SavingAccount? GetSavingsAccountBySdt(string sdt);
        
        RegularAccount? GetRegularAccountByID(int id);
        LoanAccount? GetLoanAccountByID(int id);
        SavingAccount? GetSavingsAccountByID(int id);
        BankAccount? GetByID(int id);
        void Update (BankAccount bankAccount);
        bool FreezeAccount(int accountID);
        bool UnlockAccount(int accountID);
        void AddRegular(User user);
        void AddLoan(User user);
        void AddSavings(User user);
        void AddTrans(Trans trans);
        List<BankAccount> GetAllBankAccounts();
        List<Trans>GetTransactionByDateRange(DateTime from, DateTime to);
        
        List<Trans>GetTransactionByAccountAndDate(int accountID, DateTime from, DateTime to);
    }
}
