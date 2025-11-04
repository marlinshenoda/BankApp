using BankApp1.Domain;

namespace BankApp1.Interfaces
{ 
    /// <summary>
    /// Interface containing the bankAccount methods
    /// </summary>
    public interface IBankAccount
    {
        Guid Id { get; }
        string Name { get; }    
        string ?Currency { get; }   
        decimal Balance { get;  }    
        DateTime LastUpdated { get; }
        AccountType AccountType { get; }
        Guid UserId { get; set; }
        void Withdraw(decimal amount);  
        void Deposit(decimal amount);
        void TransferTo(BankAccount to, decimal amount, string? description = null, string? category="Other");

    }
}
