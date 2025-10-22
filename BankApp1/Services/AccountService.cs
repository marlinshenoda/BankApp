
using BankApp1.Domain;
using BankApp1.Interfaces;
using BankApp1.Pages;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Xml.Linq;

namespace BankApp1.Services
{
    public class AccountService : IAccountService
    {
        
            private readonly ILocalStorageService _localStorage;
            private List<BankAccount> _accounts = new();
            private readonly ISignInService _signInService;

        public AccountService(ILocalStorageService localStorage, ISignInService signInService)
            {
            _localStorage = localStorage;
            _signInService = signInService;
        }

            private async Task LoadAsync()
            {
            var key = await GetUserStorageKeyAsync();
            _accounts = await _localStorage.GetItemAsync<List<BankAccount>>(key) ?? new List<BankAccount>();

        }
        private async Task<string> GetUserStorageKeyAsync()
        {
            var user = await _signInService.GetCurrentUserAsync();
            if (user == null)
                throw new Exception("User not signed in.");

            return  $"bank_accounts_{user.Id}";

        }

        public async Task UpdateAccountAsync(BankAccount account)
        {
            var currentUser = await _signInService.GetCurrentUserAsync();
            if (currentUser == null) return;

            var existing = currentUser.Accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existing != null)
            {
                existing.Balance = account.Balance;
            }

            await SaveAsync();
        }
        private async Task SaveAsync()
            {
            var key = await GetUserStorageKeyAsync();
            await _localStorage.SetItemAsync(key, _accounts);

          
            }
        public async Task<List<BankAccount>> GetAccountsByUserIdAsync(Guid userId)
        {
            var key = $"bank_accounts_{userId}";
            var stored = await _localStorage.GetItemAsync<List<BankAccount>>(key);
            return stored ?? new List<BankAccount>();

        }
        public async Task<IBankAccount> CreateAccountAsync(BankAccount account)
            {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.UserId == Guid.Empty)
                throw new ArgumentException("Account must have a valid UserId.");
            var key = $"bank_accounts_{account.UserId}";
            var existing = await _localStorage.GetItemAsync<List<BankAccount>>(key) ?? new List<BankAccount>();
            existing.Add(account);
            await _localStorage.SetItemAsync(key, existing);
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

        public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description)
        {
            var user = await _signInService.GetCurrentUserAsync();
            if (user == null)
                throw new InvalidOperationException("No user is signed in.");

            var accountsKey = $"bank_accounts_{user.Id}";
            var accounts = await _localStorage.GetItemAsync<List<BankAccount>>(accountsKey) ?? new List<BankAccount>();



            Console.WriteLine($"[TransferAsync] loaded {accounts.Count} accounts. ids: {string.Join(", ", accounts.Select(a => a.Id))}");

            var from = accounts.FirstOrDefault(a => a.Id == fromAccountId)
                ?? throw new KeyNotFoundException($"Source account {fromAccountId} not found in local storage.");

            var to = accounts.FirstOrDefault(a => a.Id == toAccountId)
                ?? throw new KeyNotFoundException($"Destination account {toAccountId} not found in local storage.");

            from.TransferTo(to, amount, description);

            // Save back to local storage
            await _localStorage.SetItemAsync(accountsKey, accounts);

            Console.WriteLine($"[TransferAsync] ✅ Transfer completed between {from.Id} and {to.Id}");
        }

        public async Task<List<Transaction>> GetRecentTransactionsAsync(Guid userId)
        {
            await Task.Delay(100); 
            return new List<Transaction>
    {
        new Transaction { Timestamp = DateTime.Now.AddDays(-1), Description = "Grocery Store", Amount = -150m, Status = "Success", Category = "Food" },
        new Transaction { Timestamp = DateTime.Now.AddDays(-2), Description = "Salary Deposit", Amount = 2500m, Status = "Success", Category = "Income" },
        new Transaction { Timestamp = DateTime.Now.AddDays(-3), Description = "Electric Bill", Amount = -200m, Status = "Success", Category = "Utilities" }
    };
        }


    }
}
