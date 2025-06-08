using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;
using PBL3.Models;
using PBL3.Repositories;

namespace PBL3.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepo;
        public BankAccountService(IBankAccountRepository bankAccountRepo)
        {
            _bankAccountRepo = bankAccountRepo;
        }

        public void CreateBankAccount(User user)
        {

            _bankAccountRepo.AddRegular(user);
        }
        public (bool Success, string Message) Transfer(TransferViewModel model)
        {
            RegularAccount fromAccount = GetRegularAccountByID(model.FromAccountId);
            RegularAccount toAccount = GetRegularAccountByID(model.ToAccountId);

            if (fromAccount == null)
                return (false, "Tài khoản người gửi không tồn tại.");

            if (toAccount == null)
                return (false, "Tài khoản người nhận không tồn tại.");

            if (model.Amount <= 0)
                return (false, "Số tiền phải lớn hơn 0.");

            if (model.Amount > fromAccount.Balance)
                return (false, "Số tiền chuyển lớn hơn số dư tài khoản.");

            try
            {
                fromAccount.Transfer(model.Amount, toAccount);

                var transaction = new Trans(
                    fromAccount.AccountId,
                    toAccount.AccountId,
                    fromAccount,
                    toAccount,
                    model.Amount,
                    TransactionType.Transfer,
                    model.description
                );

                _bankAccountRepo.Update(fromAccount);
                _bankAccountRepo.Update(toAccount);
                _bankAccountRepo.AddTrans(transaction);

                return (true, "Chuyển tiền thành công.");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi chuyển tiền: " + ex.Message);
            }
        }

        public double GetBalance(int accountId)
        {
            var account = _bankAccountRepo.GetByID(accountId);
            return account?.Balance ?? 0;
        }

        public RegularAccount? GetRegularAccountBySdt(string sdt)
        {
            return _bankAccountRepo.GetRegularAccountBySdt(sdt);
        }

        public LoanAccount? GetLoanAccountBySdt(string sdt)
        {
            return _bankAccountRepo.GetLoanAccountBySdt(sdt);
        }

        public SavingAccount? GetSavingAccountBySdt(string sdt)
        {
            return _bankAccountRepo.GetSavingsAccountBySdt(sdt);
        }

        public RegularAccount? GetRegularAccountByID(int id)
        {
            return _bankAccountRepo.GetRegularAccountByID(id);
        }

        public LoanAccount? GetLoanAccountByID(int id)
        {
            return _bankAccountRepo.GetLoanAccountByID(id);
        }

        public SavingAccount? GetSavingAccountByID(int id)
        {
            return _bankAccountRepo.GetSavingsAccountByID(id);
        }

        public bool IsAccountActive(string sdt)
        {
            RegularAccount acc = GetRegularAccountBySdt(sdt);
            return acc != null && acc.IsActive();
        }

        public bool FreezeAccount(int accountId)
        {
            return _bankAccountRepo.FreezeAccount(accountId);
        }

        public BankAccount? GetByID(int id)
        {
            return _bankAccountRepo.GetByID(id);
        }
        public bool UnlockAccount(int accountId)
        {
            return _bankAccountRepo.UnlockAccount(accountId);
        }

        public List<Trans> GetTransactionByDateRange(DateTime from, DateTime to)
        {
            return _bankAccountRepo.GetTransactionByDateRange(from, to);
        }
    }
 }

