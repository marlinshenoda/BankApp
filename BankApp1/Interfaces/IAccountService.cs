
using BankApp1.Domain;

namespace BankApp1.Interfaces
{
    public interface IAccountService {

        Task<IBankAccount> CreateAccountAsync(BankAccount account);
        Task<List<IBankAccount>> GetAllAccountsAsync();
        Task<IBankAccount?> GetAccountByIdAsync(Guid id);
        Task<List<BankAccount>> GetAccountsByUserIdAsync(Guid userId);
        Task<List<Transaction>> GetRecentTransactionsAsync(Guid userId);
        Task SaveAccountsAsync(User user);
        Task ApplyInterestToSavingsAsync(Guid userId);

        Task DepositAsync(Guid accountId, decimal amount, string? description = null);
        Task WithdrawAsync(Guid accountId, decimal amount, string? description = null);
        Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description = null, string? category=null);
        Task AddTransactionAsync(Guid userId, Transaction transaction);

    }
}
