namespace BankApp1.Domain
{
    public class User
    {
        public string Username { get; set; } = "";
        public string? Pin { get; set; }
        public List<BankAccount> Accounts { get; set; } = new();
    }
}
