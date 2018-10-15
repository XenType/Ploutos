﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class CreditAssetAccount : AssetAccount
    {
        public decimal CreditLine { get; set; }
        public decimal InterestRate { get; set; }
        public TimePeriod StatementDate { get; set; }
        public List<ExpenseAccount> LinkedExpenseAccounts { get; set; }

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

        public decimal CalculateFutureBalance(DateTime futureDate)
        {
            //TODO: Update when TimePeriod is implemented
            //TODO: Update when ExpenseAccount is implemented
            return 0.00M;
        }
    }
}