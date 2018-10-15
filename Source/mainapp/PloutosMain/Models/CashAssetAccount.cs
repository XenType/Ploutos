﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class CashAssetAccount : AssetAccount
    {
        public CashAssetAccount()
        {

        }
        public CashAssetAccount(AssetAccount assetAccount)
        {
            Id = assetAccount.Id;
            Name = assetAccount.Name;
            AccountType = assetAccount.AccountType;
            Balance = assetAccount.Balance;
            AssetAccountType = assetAccount.AssetAccountType;
        }
    }
}