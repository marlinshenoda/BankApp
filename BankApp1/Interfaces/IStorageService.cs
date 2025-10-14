namespace BankApp1.Interfaces
{
    public interface IStorageService
    {
        Task SaveAsync<T>(string key, T value);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
