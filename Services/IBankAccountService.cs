using PBL3.Entities;
using PBL3.Models;

namespace PBL3.Services
{
    public interface IBankAccountService
    {
        void CreateBankAccount(User user);
        RegularAccount? GetRegularAccountBySdt(string sdt);
        string? GetAccountType(string sdt, int accountID);
        (bool Success, string Message, double NewBalance) Transfer(TransferViewModel model);
        void S
    }
}
