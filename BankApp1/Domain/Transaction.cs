namespace BankApp1.Domain
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }           // positivt värde
        public decimal BalanceAfter { get; set; }     // konto-saldo efter transaktion
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public string Category { get; set; } = "";

        public Guid? FromAccountId { get; set; } 
        public Guid? ToAccountId { get; set; }
    }
}
