namespace BankApp1.Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = "";
        public string? Pin { get; set; }
        public List<BankAccount> Accounts { get; set; } = new();
    }
}
