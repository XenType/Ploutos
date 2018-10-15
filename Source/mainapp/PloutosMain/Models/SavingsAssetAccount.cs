using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class SavingsAssetAccount : AssetAccount
    {
        public decimal InterestRate { get; set; }
        public TimePeriod StatementDate { get; set; }

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