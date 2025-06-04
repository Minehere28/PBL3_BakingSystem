using Microsoft.EntityFrameworkCore;
using PBL3.Entities;

namespace PBL3.Data
{
    public class BMContext : DbContext
    {
        public BMContext(DbContextOptions<BMContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Trans> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inheritance mapping
            modelBuilder.Entity<RegularAccount>().HasBaseType<BankAccount>();
            modelBuilder.Entity<LoanAccount>().HasBaseType<BankAccount>();
            modelBuilder.Entity<SavingAccount>().HasBaseType<BankAccount>();

            // Transaction relationship
            modelBuilder.Entity<Trans>()
                .HasOne(t => t.FromAccount)
                .WithMany(a => a.SentTransactions)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trans>()
                .HasOne(t => t.ToAccount)
                .WithMany(a => a.ReceivedTransactions)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Account Discriminator
            modelBuilder.Entity<BankAccount>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<RegularAccount>("Regular")
                .HasValue<LoanAccount>("Loan")
                .HasValue<SavingAccount>("Saving");
        }
    }
}
