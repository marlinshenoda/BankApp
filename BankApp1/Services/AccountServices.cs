
using BankApp1.Domain;

namespace BankApp1.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly List<IBankAccount> _accounts = new List<IBankAccount>();

        public IBankAccount CreateAccount(string name, decimal currency, decimal intialBalance)
        {
            throw new NotImplementedException(); if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(currency.ToString()))
                throw new ArgumentException("Currency cannot be empty.");
            if (intialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.");

            var account = new BankAccount(name, currency, intialBalance);
            _accounts.Add(account);
            return account;
        }

       

        public List<IBankAccount> GetAllAccounts()
        {
            return new List<IBankAccount>(_accounts);
        }
    }
}
