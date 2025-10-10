
using BankApp1.Domain;

namespace BankApp1.Interfaces
{
    public interface IAccountServices { 
    
        IBankAccount CreateAccount (string name, string currency,decimal intialBalance, AccountType accountType);
        List<IBankAccount> GetAllAccounts ();

    }
}
