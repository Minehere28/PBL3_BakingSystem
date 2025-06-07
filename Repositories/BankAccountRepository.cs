using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PBL3.Data;
using PBL3.Entities;

namespace PBL3.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
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
        public LoanAccount? GetLoanAccountBySdt(string sdt)
        {
            return _context.BankAccounts
                .OfType<LoanAccount>()
                .FirstOrDefault(a => a.Sdt == sdt);
        }
        public SavingAccount? GetSavingsAccountBySdt(string sdt)
        {
            return _context.BankAccounts
                .OfType<SavingAccount>()
                .FirstOrDefault(a => a.Sdt == sdt);
        }
        public RegularAccount? GetRegularAccountByID(int id)
        {
            return _context.BankAccounts
                .OfType<RegularAccount>()
                .FirstOrDefault(a => a.AccountId == id);
        }
        public LoanAccount? GetLoanAccountByID(int id)
        {
            return _context.BankAccounts
                .OfType<LoanAccount>()
                .FirstOrDefault(a => a.AccountId == id);
        }
        public SavingAccount? GetSavingsAccountByID(int id)
        {
            return _context.BankAccounts
                .OfType<SavingAccount>()
                .FirstOrDefault(a => a.AccountId == id);
        }

        ////Sửa lại logic phương thức này
        //public string? GetAccountType(string sdt, int accountId)
        //{
        //    return _context.BankAccounts
        //        .Where(b => b.Sdt == sdt && b.AccountId == accountId)
        //        .Select(b => EF.Property<string>(b, "AccountType"))
        //        .FirstOrDefault();
        //}
        public void Update(BankAccount bankAccount)
        {
            _context.BankAccounts.Update(bankAccount);
            _context.SaveChanges();
        }
        public BankAccount? GetByID(int id)
        {
            return _context.BankAccounts.FirstOrDefault(a => a.AccountId == id);
        }
        public void AddTrans(Trans trans)
        {
            _context.Transactions.Add(trans);
            _context.SaveChanges();
        }

        public void AddLoan(User user)
        {
            var account = new LoanAccount
            {
                Sdt = user.Sdt,
                user = user,
                AccountId = BankAccount.GenerateAccountId(_context)
                // Có thể thêm các thuộc tính đặc thù của LoanAccount nếu cần
            }
            ;
            _context.BankAccounts.Add(account);
            _context.SaveChanges();
        }

        public void AddSavings(User user)
        {
            var account = new SavingAccount
            {
                Sdt = user.Sdt,
                user = user,
                AccountId = BankAccount.GenerateAccountId(_context)
                // Có thể thêm các thuộc tính đặc thù của SavingAccount nếu cần
            };
            _context.BankAccounts.Add(account);
            _context.SaveChanges();
        }

        public List<BankAccount> GetAllBankAccounts()
        {
            return _context.BankAccounts.ToList();
        }
    }
}
