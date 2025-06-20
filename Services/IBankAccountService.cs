﻿using PBL3.Entities;
using PBL3.Models;

namespace PBL3.Services
{
    public interface IBankAccountService
    {
        void CreateBankAccount(User user);
        //Service thì không có phương thức này!!!
        RegularAccount? GetRegularAccountBySdt(string sdt);
        LoanAccount? GetLoanAccountBySdt(string sdt);
        SavingAccount? GetSavingAccountBySdt(string sdt);
        RegularAccount? GetRegularAccountByID(int id);
        LoanAccount? GetLoanAccountByID(int id);
        SavingAccount? GetSavingAccountByID(int id);
        List<BankAccount> GetAllBankAccounts();

        (bool Success, string Message) Transfer(TransferViewModel model);
        (bool Success, string Message) Deposit(DepositViewModel model);
        (bool Success, string Message) Withdraw(WithdrawViewModel model);
        double GetBalance(int accountId);
        bool IsAccountActive(string sdt);
        bool FreezeAccount(int accountId);
        BankAccount? GetByID(int id);
        bool UnlockAccount(int accountId);
        List<Trans> GetTransactionByDateRange(DateTime from, DateTime to);
        List<Trans> GetTransactionByAccountAndDate(int accountId, DateTime from, DateTime to);
    }
}
