
namespace PloutosMain.Models
{
    public class AssetAccount : Account
    {
        public decimal Balance { get; set; }
        public AssetAccountType AssetAccountType { get; set; }

        public AssetAccount()
        {

        }
        public AssetAccount(Account account)
        {
            Id = account.Id;
            Name = account.Name;
            AccountType = account.AccountType;
        }
    }
    public enum AssetAccountType { Cash, Credit, Savings }
}