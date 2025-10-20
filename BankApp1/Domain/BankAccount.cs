using System.Text.Json.Serialization;

namespace BankApp1.Domain
{
    public class BankAccount : IBankAccount
    {
        [JsonInclude]

        public Guid Id { get; } = Guid.NewGuid();

        public string Name { get; set; }

        public string? Currency { get; set; } = "SEK";
        [JsonInclude]
        public decimal Balance { get; internal set; }

        public DateTime LastUpdated { get; private set; }
        public Guid UserId { get; set; }
        public AccountType AccountType { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public BankAccount() { }

        public BankAccount(string name, string currency, decimal initialBalance , AccountType accountType , Guid userId)
        {
            Name = name;
            Currency = currency;
            Balance = initialBalance;
            AccountType = accountType;
            UserId = userId;
            LastUpdated = DateTime.Now;

        }
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");

            Balance += amount;
            LastUpdated = DateTime.Now;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive.");

            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds.");

            Balance -= amount;
            LastUpdated = DateTime.Now;
        }
        public void TransferTo(BankAccount to, decimal amount, string? description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be positive.");

            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds.");

            //  från 
            Balance -= amount;
            LastUpdated = DateTime.UtcNow;

            Transactions.Add(new Transaction
            {
                Type = TransactionType.Transfer,
                Amount = amount,
                BalanceAfter = Balance,
                Description = description ?? $"Transfer to {to.Name}",
                RelatedAccountId = to.Id
            });

            // till
            to.Balance += amount;
            to.LastUpdated = DateTime.UtcNow;

            to.Transactions.Add(new Transaction
            {
                Type = TransactionType.Transfer,
                Amount = amount,
                BalanceAfter = to.Balance,
                Description = description ?? $"Transfer from {Name}",
                RelatedAccountId = Id
            });
        }

    }
}
