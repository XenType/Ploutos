
namespace PloutosMain.Models
{
    public class SavingsAssetAccount : AssetAccount
    {
        public decimal InterestRate { get; set; }
        public TimePeriod StatementTimePeriod { get; set; }

        public SavingsAssetAccount()
        {

        }
        public SavingsAssetAccount(AssetAccount assetAccount)
        {
            Id = assetAccount.Id;
            Name = assetAccount.Name;
            AccountType = assetAccount.AccountType;
            Balance = assetAccount.Balance;
            AssetAccountType = assetAccount.AssetAccountType;
        }
    }
}