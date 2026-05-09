using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace EF_core_task_4.Entites
{
    public enum AccountType
    {
        Savings,
        Current,
        FixedDeposit
    }

    public enum AccountStatus
    {
        Active,
        Suspended,
        Closed
    }

    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = null!; 
        public decimal Balance { get; set; }
        public AccountType Type { get; set; }
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public string Currency { get; set; } = "EGP";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        // Navigation Properties
        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; } = new HashSet<CustomerAccount>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
    public class CustomerAccount
    {
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        public int AccountId { get; set; }
        public virtual Account Account { get; set; } = null!;

        public bool IsPrimaryOwner { get; set; } = true;
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
