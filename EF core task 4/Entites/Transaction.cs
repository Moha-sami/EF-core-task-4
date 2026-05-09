using System;
using System.Collections.Generic;
using System.Text;

namespace EF_core_task_4.Entites
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut
    }

    public class Transaction
    {
        public long Id { get; set; }
        public string TransactionReference { get; set; } = null!; 
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }

        // Foreign Keys
        public int AccountId { get; set; }
        public virtual Account Account { get; set; } = null!;
    }
}
