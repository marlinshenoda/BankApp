using BankApp1.Domain;

namespace BankApp1.Interfaces
{
    public interface ISignInService
    {
        Task<User?> SignInAsync(string username, string pin);
        Task SignOutAsync();
        Task<User?> GetCurrentUserAsync();
        bool IsSignedIn { get; }
        User? CurrentUser { get; }
        event Action? OnChange;

    }
}
