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
        public RegularAccount? GetRegularAccountBySdt(string sdt)
        {
            return _bankAccountRepo.GetRegularAccountBySdt(sdt);
        }
        public string? GetAccountType(string sdt, int accountId)
        {
            return _bankAccountRepo.GetAccountType(sdt, accountId);
        }

        public (bool Success, string Message, double NewBalance) Transfer(TransferViewModel model)
        {
            var fromAccount = _bankAccountRepo.GetBankAccountByID(model.FromAccountId);
            var toAccount = _bankAccountRepo.GetBankAccountByID(model.ToAccountId);

            if(toAccount == null)
            return (false, "Tài khoản người nhận không tồn tại.", 0);

            if (fromAccount == null)
                return (false, "Tài khoản người gửi không tồn tại.", 0);

            if (model.Amount <= 0)
                return (false, "Số tiền phải lớn hơn 0.", fromAccount.Balance);

            if (model.Amount > fromAccount.Balance)
                return (false, "Số tiền chuyển lớn hơn số dư tài khoản.", fromAccount.Balance);
            try
            {

            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return (false, "Có lỗi xảy ra khi chuyển tiền: " + inner, fromAccount?.Balance ?? 0);
            }
            catch (Exception ex)
            {
                return (false, "Có lỗi xảy ra khi chuyển tiền: " + ex.Message, fromAccount?.Balance ?? 0);
            }
        }
    }
 }

