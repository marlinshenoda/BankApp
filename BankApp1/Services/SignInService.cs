using BankApp1.Domain;
using System.Collections.Generic;
using System.Text.Json;

namespace BankApp1.Services
{
    public class SignInService : ISignInService
    {
        private const string CurrentUserKey = "current_user";
        private readonly IStorageService _storage;
        private User? _currentUser;
        public event Func<Task>? OnChange;
        private bool _isSignedIn = false;

        public SignInService(IStorageService storage)
        {
            _storage = storage;
        }
        public async Task NotifyChange()
        {
            if (OnChange != null) await OnChange.Invoke();
        }
      //  public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public bool IsSignedIn => _isSignedIn;
        public User? CurrentUser => _currentUser;

        public async Task<User?> SignInAsync(string username, string pin)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username required.");

            // Use username as the key for now
            var key = $"bank_accounts_{username.ToLower()}";
            var storedUser = await _storage.GetAsync<User>(key);
            User user;

            if (storedUser == null)
            {
                // New user → create one and give it a unique ID
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Pin = pin,
                    Accounts = new List<BankAccount>()
                };

                // Save with username key
                await _storage.SaveAsync(key, user);

                // ALSO create an empty account list entry
                var accountsKey = $"bank_accounts_{user.Id}";
                await _storage.SaveAsync(accountsKey, user.Accounts);
            }
            else
            {
                // Existing user → check PIN
                user = storedUser;
                if (!string.IsNullOrEmpty(user.Pin) && user.Pin != pin)
                    throw new UnauthorizedAccessException("Invalid PIN.");

                // Load their accounts using their unique ID
                var accountsKey = $"bank_accounts_{user.Id}";
                var accounts = await _storage.GetAsync<List<BankAccount>>(accountsKey);
                user.Accounts = accounts ?? new List<BankAccount>();
            }

            _currentUser = user;
            _isSignedIn = true;

            // Save "currently signed-in user" globally
            await _storage.SaveAsync(CurrentUserKey, user);

            NotifyStateChanged();
            if (OnChange != null)
                await OnChange.Invoke();

            return _currentUser;
        }


        public async Task SignOutAsync()
        {
            _currentUser = null;
            _isSignedIn = false;

            await _storage.RemoveAsync(CurrentUserKey);
            NotifyStateChanged();
        }

        public async Task SetCurrentUserAsync(User user)
        {
            _currentUser = user;
            await SaveUserStateAsync();

            if (OnChange != null)
                await OnChange.Invoke();
        }
        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
            {

                return _currentUser; }

            var storedUser = await _storage.GetAsync<User>(CurrentUserKey);
            if (storedUser == null) return null;

            _currentUser = storedUser;

            // Load bank accounts if exist
            var accountsKey = $"bank_accounts_{_currentUser.Id}";
            var accountsJson = await _storage.GetAsync<string>(accountsKey);

            _currentUser.Accounts = string.IsNullOrWhiteSpace(accountsJson)
                ? new List<BankAccount>()
                : JsonSerializer.Deserialize<List<BankAccount>>(accountsJson) ?? new List<BankAccount>();
            _isSignedIn = false;

            return _currentUser;
        }
        private async Task SaveUserStateAsync()
        {
            if (_currentUser == null) return;



            var accountsKey = $"bank_accounts_{_currentUser.Id}";


            await _storage.SaveAsync(accountsKey, _currentUser.Accounts ?? new List<BankAccount>());
        }

    }
}


