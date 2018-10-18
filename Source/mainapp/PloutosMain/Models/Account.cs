
namespace PloutosMain.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccountType AccountType { get; set; }
    }

    public enum AccountType { Income, Expense, Asset }
}