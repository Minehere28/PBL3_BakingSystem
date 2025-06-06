using PBL3.Entities;

namespace PBL3.Repositories
{
    public interface IBankAccountRepository
    {
        RegularAccount? GetRegularAccountBySdt(string sdt);
        BankAccount? GetBankAccountByID(int id);
        public string? GetAccountType(string sdt, int accountID);
        void Update (BankAccount bankAccount);
        void AddRegular(User user);
        void AddTrans(Trans trans);
    }
}
