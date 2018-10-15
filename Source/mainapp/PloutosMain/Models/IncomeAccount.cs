using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class IncomeAccount : Account
    {
        public IncomeAccountType IncomeAccountType { get; set; }
    }
    public enum IncomeAccountType { Salary, Hourly, Tipped, Commission }
}