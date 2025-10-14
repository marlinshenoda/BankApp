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

        public User? CurrentUser => throw new NotImplementedException();

      

        public async Task<User?> SignInAsync(string username, string pin)
        {
            var user = await SignInAsync(username, pin);
            _currentUser = user;

            // Save current user
            await _storage.SaveAsync(CurrentUserKey, user);

            NotifyStateChanged();
            return user;
        }


        public async Task SignOutAsync()
        {
            _currentUser = null;
            await _storage.RemoveAsync(CurrentUserKey);
            NotifyStateChanged();
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            // Get the current user key from storage
            var storedUser = await _storage.GetAsync<User>(CurrentUserKey);
            if (storedUser == null)
                return null;

            _currentUser = storedUser;

            // Optionally, load bank accounts for the user
            var accountsKey = $"bankapp_state_{_currentUser.Username.ToLower()}_accounts";
            var accountsJson = await _storage.GetAsync<string>(accountsKey);

            if (!string.IsNullOrWhiteSpace(accountsJson))
            {
                try
                {
                    _currentUser.Accounts = JsonSerializer.Deserialize<List<BankAccount>>(accountsJson) ?? new List<BankAccount>();
                }
                catch
                {
                    _currentUser.Accounts = new List<BankAccount>();
                }
            }
            else
            {
                _currentUser.Accounts = new List<BankAccount>();
            }

            return _currentUser;
        }
     

        private async Task SaveUserStateAsync(User user)
        {
            var key = $"bankapp_state_{user.Username.ToLower()}";
            var json = JsonSerializer.Serialize(user);
            await _storage.SaveAsync(key, json);
        }
    }
}

