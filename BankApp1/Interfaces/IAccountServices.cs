
namespace BankApp1.Interfaces
{
    public interface IAccountServices { 
    
        IBankAccount CreateAccount (string name, string currency,decimal intialBalance);
        List<IBankAccount> GetAllAccounts ();

    }
}
