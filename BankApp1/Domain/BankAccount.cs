
namespace BankApp1.Domain
{
    public class BankAccount : IBankAccount
    {
        public Guid Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public decimal Currency => throw new NotImplementedException();

        public decimal Balance => throw new NotImplementedException();

        public DateTime LastUpdated => throw new NotImplementedException();

        public void Deposit(decimal amount)
        {
            throw new NotImplementedException();
        }

        public void Withdraw(decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}
