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
                )
                {
                    SenderBalanceAfter = fromAccount.Balance,
                    ReceiverBalanceAfter = toAccount.Balance
                }
                ;
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

        public List<Trans> GetTransactionByAccountAndDate(int accountId, DateTime from, DateTime to)
        {
            return _bankAccountRepo.GetTransactionByAccountAndDate(accountId, from, to);
        }

        public List<BankAccount> GetAllBankAccounts()
        {
            return _bankAccountRepo.GetAllBankAccounts();
        }

        public (bool Success, string Message) Deposit(DepositViewModel model)
        {
            var account = GetRegularAccountByID(model.AccountId);

            if (account == null)
                return (false, "❌ Tài khoản không tồn tại.");

            if (model.Amount <= 0)
                return (false, "❌ Số tiền nạp phải lớn hơn 0.");

            try
            {
                account.ReceiveTransfer(model.Amount);

                // Tạo giao dịch kiểu "nạp tiền"
                //var transaction = new Trans
                //{
                //    FromAccountId = 00000000, // admin mặc định
                //    ToAccountId = model.AccountId,
                //    Amount = model.Amount,
                //    Description = $"Nạp tiền cho {model.AccountId}",
                //    TransactionDate = DateTime.Now,
                //    Type = TransactionType.Deposit,
                //    ReceiverBalanceAfter = account.Balance,
                //    SenderBalanceAfter = null // Admin không có tài khoản ngân hàng
                //};
                var transaction = new Trans(
                    00000000,
                    account.AccountId,
                    null,
                    account,
                    model.Amount,
                    TransactionType.Deposit,
                    $"Nạp tiền cho {model.AccountId}"
                )
                {
                    SenderBalanceAfter = null,
                    ReceiverBalanceAfter = account.Balance
                };
                _bankAccountRepo.Update(account);
                _bankAccountRepo.AddTrans(transaction);

                return (true, $"✅ Nạp {model.Amount:N0} VND vào tài khoản {model.AccountId} thành công.");
            }
            catch (Exception ex)
            {
                return (false, "❌ Gặp lỗi khi nạp tiền: " + ex.Message);
            }
        }
        public (bool Success, string Message) Withdraw(WithdrawViewModel model)
        {
            var account = GetRegularAccountByID(model.AccountId);

            if (account == null)
                return (false, "❌ Tài khoản không tồn tại.");

            if (model.Amount <= 0)
                return (false, "❌ Số tiền rút phải lớn hơn 0.");

            try
            {
                account.Withdraw(model.Amount);

                // Tạo giao dịch kiểu "nạp tiền"
                //var transaction = new Trans
                //{
                //    FromAccountId = 00000000, // admin mặc định
                //    ToAccountId = model.AccountId,
                //    Amount = model.Amount,
                //    Description = $"Nạp tiền cho {model.AccountId}",
                //    TransactionDate = DateTime.Now,
                //    Type = TransactionType.Deposit,
                //    ReceiverBalanceAfter = account.Balance,
                //    SenderBalanceAfter = null // Admin không có tài khoản ngân hàng
                //};
                Trans transaction = new Trans(
                    account.AccountId,
                    00000000,
                    account,
                    null,
                    model.Amount,
                    TransactionType.Withdrawal,
                    $"Rút tiền cho {model.AccountId}"
                )
                {
                    SenderBalanceAfter = account.Balance,
                    ReceiverBalanceAfter = null
                };
                _bankAccountRepo.Update(account);
                _bankAccountRepo.AddTrans(transaction);

                return (true, $"✅ Rút {model.Amount:N0} VND cho tài khoản {model.AccountId} thành công.");
            }
            catch (Exception ex)
            {
                return (false, "❌ Gặp lỗi khi rút tiền: " + ex.Message);
            }
        }
    }
 }

