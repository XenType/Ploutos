
namespace PloutosMain.Models
{
    public class AssetAccount : Account
    {
        public decimal Balance { get; set; }
        public AssetAccountType AssetAccountType { get; set; }

        public AssetAccount()
        {
            AccountType = AccountType.Asset;
        }
        public AssetAccount(Account account)
        {
            Id = account.Id;
            Name = account.Name;
            AccountType = AccountType.Asset;
        }
    }
    public enum AssetAccountType { Cash, Credit, Savings }
}