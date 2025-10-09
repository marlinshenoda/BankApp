
namespace BankApp1.Interfaces
{
    public interface IAccountServices { 
    
        IBankAccount CreateAccount (string name, decimal currency,decimal intialBalance);
        List<IBankAccount> GetAllAccounts ();

    }
}
