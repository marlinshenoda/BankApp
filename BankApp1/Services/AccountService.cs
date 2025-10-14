
using BankApp1.Domain;
using BankApp1.Pages;
using Blazored.LocalStorage;
using System.Text.Json;

namespace BankApp1.Services
{
    public class AccountService : IAccountService
    {
        
            private const string StorageKey = "bank_accounts";
            private readonly ILocalStorageService _localStorage;
            private List<BankAccount> _accounts = new();

            public AccountService(ILocalStorageService localStorage)
            {
                _localStorage = localStorage;
            }

            private async Task LoadAsync()
            {
                var stored = await _localStorage.GetItemAsync<List<BankAccount>>(StorageKey);
                _accounts = stored ?? new List<BankAccount>();
            }

            private async Task SaveAsync()
            {
                await _localStorage.SetItemAsync(StorageKey, _accounts);
            }

            public async Task<IBankAccount> CreateAccountAsync(string name, string currency, decimal initialBalance, AccountType accountType)
            {
                await LoadAsync();
                var account = new BankAccount(name, currency, initialBalance, accountType);
                _accounts.Add(account);
                await SaveAsync();
                return account;
            }

            public async Task<List<IBankAccount>> GetAllAccountsAsync()
            {
                await LoadAsync();
                return _accounts.Cast<IBankAccount>().ToList();
            }

            public async Task<IBankAccount?> GetAccountByIdAsync(Guid id)
            {
                await LoadAsync();
                return _accounts.FirstOrDefault(a => a.Id == id);
            }

            public async Task DepositAsync(Guid accountId, decimal amount, string? description = null)
            {
                await LoadAsync();
                var account = _accounts.FirstOrDefault(a => a.Id == accountId)
                    ?? throw new KeyNotFoundException("Account not found.");
                account.Deposit(amount);
                await SaveAsync();
            }

            public async Task WithdrawAsync(Guid accountId, decimal amount, string? description = null)
            {
                await LoadAsync();
                var account = _accounts.FirstOrDefault(a => a.Id == accountId)
                    ?? throw new KeyNotFoundException("Account not found.");
                account.Withdraw(amount);
                await SaveAsync();
            }

            public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description = null)
            {
                await LoadAsync();
                if (fromAccountId == toAccountId)
                    throw new ArgumentException("Cannot transfer to the same account.");

                var from = _accounts.First(a => a.Id == fromAccountId);
                var to = _accounts.First(a => a.Id == toAccountId);
                from.TransferTo(to, amount, description);
                await SaveAsync();
            }

        }
}
