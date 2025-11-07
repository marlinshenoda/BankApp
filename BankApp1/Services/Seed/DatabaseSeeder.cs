using BankApp1.Domain;

namespace BankApp1.Services.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IStorageService storage)
        {
            // Test User 1
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Username = "test1",
                Pin = "1111",
                Accounts = new List<BankAccount>()
            };

            user1.Accounts.Add(new BankAccount("Checking Account", "SEK", 1500m, AccountType.Deposit, user1.Id));
            user1.Accounts.Add(new BankAccount("Savings Account", "SEK", 5000m, AccountType.Saving, user1.Id));

            await storage.SaveAsync($"bank_accounts_{user1.Username.ToLower()}", user1);
            await storage.SaveAsync($"bank_accounts_{user1.Id}", user1.Accounts);


            // Test User 2
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Username = "test2",
                Pin = "2222",
                Accounts = new List<BankAccount>()
            };

            user2.Accounts.Add(new BankAccount("Main Account", "SEK", 900m, AccountType.Deposit, user2.Id));
            user2.Accounts.Add(new BankAccount("Savings Account", "SEK", 900m, AccountType.Saving, user2.Id));

            await storage.SaveAsync($"bank_accounts_{user2.Username.ToLower()}", user2);
            await storage.SaveAsync($"bank_accounts_{user2.Id}", user2.Accounts);
        }

    }
}
