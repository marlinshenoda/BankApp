namespace BankApp1.Interfaces
{ 
    /// <summary>
    /// Interface containing the bankAccount methods
    /// </summary>
    public interface IBankAccount
    {
        Guid Id { get; }
        string Name { get; }    
        decimal Currency { get; }   
        public decimal Balance { get;  }    
        DateTime LastUpdated { get; }

        void Withdraw(decimal amount);  
        void Deposit(decimal amount);
    }
}
