using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;

namespace PBL3.Repositories
{
    public class BankAccountRepository:IBankAccountRepository
    {
        private readonly BMContext _context;
        public BankAccountRepository(BMContext context)
        {
            _context = context;
        }

        public void AddRegular(User user)
        {
            var account = new RegularAccount
            {
                Sdt = user.Sdt,
                user = user,
                AccountId = BankAccount.GenerateAccountId(_context)
            };
            _context.BankAccounts.Add(account);
            _context.SaveChanges();
        }

        public RegularAccount? GetRegularAccountBySdt(string sdt)
        {
            return _context.BankAccounts
                .OfType<RegularAccount>()
                .FirstOrDefault(a => a.Sdt == sdt);
        }
        public string? GetAccountType(string sdt, int accountId)
        {
            return _context.BankAccounts
                .Where(b => b.Sdt == sdt && b.AccountId == accountId)
                .Select(b => EF.Property<string>(b, "AccountType"))
                .FirstOrDefault();
        }
        public void Update(BankAccount bankAccount)
        {
            _context.BankAccounts.Update(bankAccount);
            _context.SaveChanges();
        }
        public BankAccount? GetBankAccountByID(int id)
        {
            return _context.BankAccounts.FirstOrDefault(a=>a.AccountId == id);
        }
        public void AddTrans(Trans trans)
        {
            _context.Transactions.Add(trans);
            _context.SaveChanges();
        }
        
    }
}
