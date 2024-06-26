﻿
namespace PloutosMain.Models
{
    public class CashAssetAccount : AssetAccount
    {
        public CashAssetAccount()
        {
            AssetAccountType = AssetAccountType.Cash;
        }
        public CashAssetAccount(AssetAccount assetAccount)
        {
            Id = assetAccount.Id;
            Name = assetAccount.Name;
            AccountType = assetAccount.AccountType;
            Balance = assetAccount.Balance;
            AssetAccountType = AssetAccountType.Cash;
        }
    }
}