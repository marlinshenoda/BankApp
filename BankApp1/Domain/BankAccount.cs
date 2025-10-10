namespace BankApp1.Domain
{
    public class BankAccount : IBankAccount
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Currency { get; set; }

        public decimal Balance { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public AccountType AccountType { get; set; }    

        public BankAccount(string name, string currency, decimal initialBalance , AccountType accountType)
        {
            Name = name;
            Currency = currency;
            Balance = initialBalance;
            AccountType = accountType;
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
    }
}
