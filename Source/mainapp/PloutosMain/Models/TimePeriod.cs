using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Models
{
    public class TimePeriod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastOccurance { get; set; }
        public PeriodMethod PeriodMethod { get; set; }
        public PeriodType PeriodType { get; set; }
        public int PeriodValue { get; set; }
        public int OwnerAccountId { get; set; } // to make sure it is only edited from the original, others can copy or link to this TP
        public List<int> LinkedAccountList { get; set; }
        
        public TimePeriod()
        {
            LinkedAccountList = new List<int>();
        }
    }
    public enum PeriodMethod { EveryXUnits, SameXofUnit }
    public enum PeriodType { Day, Week, Month, Quarter, Year }
}