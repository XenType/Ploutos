using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class CreditAssetAccount : AssetAccount
    {
        public int LinkedExpenseAccountId { get; set; }

        public CreditAssetAccount()
        {

        }
        public CreditAssetAccount(AssetAccount assetAccount)
        {
            Id = assetAccount.Id;
            Name = assetAccount.Name;
            AccountType = assetAccount.AccountType;
            Balance = assetAccount.Balance;
            AssetAccountType = assetAccount.AssetAccountType;
        }
    }
}