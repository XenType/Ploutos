using System;

namespace PloutosMain.Models
{
    public class SavingsAssetAccount : AssetAccount
    {
        public decimal InterestRate { get; set; }
        public TimePeriod StatementTimePeriod { get; set; }

        public SavingsAssetAccount()
        {
            AssetAccountType = AssetAccountType.Savings;
        }
        public SavingsAssetAccount(AssetAccount assetAccount)
        {
            Id = assetAccount.Id;
            Name = assetAccount.Name;
            AccountType = assetAccount.AccountType;
            Balance = assetAccount.Balance;
            AssetAccountType = AssetAccountType.Savings;
        }
        public decimal CalculateFutureBalance(DateTime futureDate)
        {
            //TODO: Update when TimePeriod & ExpenseAccount are both implemented
            return 0.00M;
        }

    }
}