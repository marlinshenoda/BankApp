
using BankApp1.Domain;
using BankApp1.Interfaces;
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
            private const string TransactionsKey = "recentTransactions";

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
        public async Task ApplyInterestToSavingsAsync(Guid userId)
        {
            var user = await _signInService.GetCurrentUserAsync(); // or however you retrieve the user
            if (user == null || user.Id != userId) return;

            foreach (var account in user.Accounts)
            {
                // Apply interest only to savings accounts
                if (account.AccountType == AccountType.Saving ||
                    account.AccountType.ToString().ToLower().Contains("saving"))
                {
                    account.ApplyInterest();
                }
            }

            await SaveAccountsAsync(user); // persist updated balances
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
        public async Task SaveAccountsAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var key = $"bank_accounts_{user.Id}";
            var json = JsonSerializer.Serialize(user.Accounts);
            await _localStorage.SetItemAsStringAsync(key, json);

            Console.WriteLine($"[DEBUG] Saved {user.Accounts.Count} accounts for user {user.Username}");
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
            var user = await _signInService.GetCurrentUserAsync();
            if (user == null)
                throw new InvalidOperationException("No signed-in user found.");

            var accountKey = $"bank_accounts_{user.Id}";
            var accounts = await _localStorage.GetItemAsync<List<BankAccount>>(accountKey) ?? new List<BankAccount>();

            var account = accounts.FirstOrDefault(a => a.Id == accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            account.Deposit(amount);
            await _localStorage.SetItemAsync(accountKey, accounts);
            // Log transaction
            await AddTransactionAsync(user.Id, new Transaction
            {
                Timestamp = DateTime.Now,
                Description = description ?? "Deposit",
                Amount = amount,
                Status = "Success",
                Category = "Deposit"
            });
        }

            public async Task WithdrawAsync(Guid accountId, decimal amount, string? description = null)
            {
            var user = await _signInService.GetCurrentUserAsync();
            if (user == null)
                throw new InvalidOperationException("No signed-in user found.");

            var accountKey = $"bank_accounts_{user.Id}";
            var accounts = await _localStorage.GetItemAsync<List<BankAccount>>(accountKey) ?? new List<BankAccount>();

            var account = accounts.FirstOrDefault(a => a.Id == accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            account.Withdraw(amount);
            await _localStorage.SetItemAsync(accountKey, accounts);
            await AddTransactionAsync(user.Id, new Transaction
            {
                Timestamp = DateTime.Now,
                Description = description ?? "Withdrawal",
                Amount = -amount,
                Status = "Success",
                Category = "Withdrawal"
            });
        }

        public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description, string? category )
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

            from.TransferTo(to, amount, description,category);

            // Save back to local storage
            await _localStorage.SetItemAsync(accountsKey, accounts);
            Console.WriteLine($"[TransferAsync] Category used: {category}");

            Console.WriteLine($"[TransferAsync] ✅ Transfer completed between {from.Id} and {to.Id}");
            // Record both sides of the transfer
            await AddTransactionAsync(from.UserId, new Transaction
            {
                Timestamp = DateTime.Now,
                Description = description ?? $"Transfer to {to.Name}",
                Amount = -amount,
                Status = "Success",
                Category = category ?? "Other"
            });

            await AddTransactionAsync(to.UserId, new Transaction
            {
                Timestamp = DateTime.Now,
                Description = description ?? $"Transfer from {from.Name}",
                Amount = amount,
                Status = "Success",
                Category = category ?? "Other"
            });
        }
        public async Task AddTransactionAsync(Guid userId, Transaction transaction)
        {
            var key = $"recentTransactions_{userId}";
            var transactions = await _localStorage.GetItemAsync<List<Transaction>>(key) ?? new List<Transaction>();
            transactions.Insert(0, transaction); // newest first
            await _localStorage.SetItemAsync(key, transactions);
        }

        public async Task<List<Transaction>> GetRecentTransactionsAsync(Guid userId)
        {
            var key = $"recentTransactions_{userId}";
            var transactions = await _localStorage.GetItemAsync<List<Transaction>>(key);
            return transactions ?? new List<Transaction>();
        }



    }
}
