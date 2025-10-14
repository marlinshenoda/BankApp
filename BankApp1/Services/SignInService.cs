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

        public SignInService(IStorageService storage)
        {
            _storage = storage;
        }

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public bool IsSignedIn => _currentUser != null;
        public User? CurrentUser => _currentUser;

        public async Task<User?> SignInAsync(string username, string pin)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username required.");

            var key = $"bankapp_state_{username.ToLower()}";
            var storedUser = await _storage.GetAsync<User>(key);
            User user;

            if (storedUser == null)
            {
                // New user
                user = new User { Username = username, Pin = pin, Accounts = new List<BankAccount>() };
                await _storage.SaveAsync(key, user);
            }
            else
            {
                // Existing user — check PIN
                user = storedUser;
                if (!string.IsNullOrEmpty(user.Pin) && user.Pin != pin)
                    throw new UnauthorizedAccessException("Invalid PIN.");
            }

            _currentUser = user;
            await _storage.SaveAsync(CurrentUserKey, user);
            NotifyStateChanged();

            return _currentUser;
        }

        public async Task SignOutAsync()
        {
            _currentUser = null;
            await _storage.RemoveAsync(CurrentUserKey);
            NotifyStateChanged();
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null) return _currentUser;

            var storedUser = await _storage.GetAsync<User>(CurrentUserKey);
            if (storedUser == null) return null;

            _currentUser = storedUser;

            // Load bank accounts if exist
            var accountsKey = $"bankapp_state_{_currentUser.Username.ToLower()}_accounts";
            var accountsJson = await _storage.GetAsync<string>(accountsKey);

            _currentUser.Accounts = string.IsNullOrWhiteSpace(accountsJson)
                ? new List<BankAccount>()
                : JsonSerializer.Deserialize<List<BankAccount>>(accountsJson) ?? new List<BankAccount>();

            return _currentUser;
        }
    }
}


