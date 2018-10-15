using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class SavingsAssetAccount : AssetAccount
    {
        public decimal InterestRate { get; set; }
        public TimePeriod TimePeriod { get; set; }
    }
}