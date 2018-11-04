using System;

namespace PloutosMain.Models
{
    public class CreditAssetAccount : AssetAccount
    {
        public decimal CreditLine { get; set; }
        public decimal CreditUsed => CreditLine - Balance;
        public decimal InterestRate { get; set; }
        public TimePeriod StatementTimePeriod { get; set; }
        
        public CreditAssetAccount()
        {
            AssetAccountType = AssetAccountType.Credit;
        }
        public CreditAssetAccount(AssetAccount assetAccount)
        {
            Id = assetAccount.Id;
            Name = assetAccount.Name;
            AccountType = assetAccount.AccountType;
            Balance = assetAccount.Balance;
            AssetAccountType = AssetAccountType.Credit;
        }
        public decimal CalculateFutureBalance(DateTime futureDate)
        {
            //TODO: Update when TimePeriod & ExpenseAccount are both implemented
            return 0.00M;
        }
        
    }
}