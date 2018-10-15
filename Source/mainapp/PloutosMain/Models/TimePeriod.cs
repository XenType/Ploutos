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
        //figure out flexible way to calculate time periods
        public int OwnerAccountId { get; set; } // to make sure it is only edited from the original, others can copy or link to this TP
    }
}