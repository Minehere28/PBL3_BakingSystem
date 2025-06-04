using PBL3.Data;
using PBL3.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BankAccount
{

    // ==== Properties ====

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int AccountId { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative.")]
    public double Balance { get; set; } = 0;

    public bool IsFrozen { get; set; } = false;

    public bool IsFlagged { get; set; } = false;

    // ==== Relationships ====
    [Required]
    public string Sdt { get; set; }

    [ForeignKey("Sdt")]
    public virtual User user { get; set; }

    public virtual ICollection<Trans> SentTransactions { get; set; }

    public virtual ICollection<Trans> ReceivedTransactions { get; set; }

    // ==== Constructors ====

    public BankAccount()
    {
        CreatedDate = DateTime.Now;

        SentTransactions = new HashSet<Trans>();
        ReceivedTransactions = new HashSet<Trans>();
    }

    // ==== Methods ====

    public void Freeze() => IsFrozen = true;
    public void UnFreeze() => IsFrozen = false;

    public void Deposit(double amount)
    {
        if (amount > 0)
            Balance += amount;
    }

    public void ReceiveTransfer(double amount)
    {
        if (amount > 0)
            Balance += amount;
    }

    public static int GenerateAccountId(BMContext bmco)
    {
        var existingIds = bmco.BankAccounts.Select(a => a.AccountId).ToHashSet();

        var rand = new Random();
        int id;
        do
        {
            id = rand.Next(10000000, 99999999); // 8-digit ID
        } while (existingIds.Contains(id));
        return id;
    }

    public static string GetCurrentDate()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }

    // ==== Abstract Methods ====

    public abstract void DisplayInfo();
    public abstract string GetData();
    public abstract string GetAccountType();
    public abstract bool Withdraw(double amount);
    public abstract bool Transfer(double amount, BankAccount targetAccount);
    public abstract void MonthlyUpdate();
}
