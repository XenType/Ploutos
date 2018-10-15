using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class ExpenseAccount : Account
    {
        public int LinkedAssetAccountId { get; set; }
    }
}