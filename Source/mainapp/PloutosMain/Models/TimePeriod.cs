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
                
        private List<int> initialLinkedAccountList;

        public TimePeriod()
        {
            LinkedAccountList = new List<int>();
            initialLinkedAccountList = new List<int>();
        }
        public void EstablishInitialLinkedAccountList(List<int> accountList)
        {
            if (initialLinkedAccountList.Count == 0)
            {
                initialLinkedAccountList = new List<int>();
                foreach (int entry in accountList)
                {
                    if (!LinkedAccountList.Contains(entry))
                        LinkedAccountList.Add(entry);
                    if (!initialLinkedAccountList.Contains(entry))
                        initialLinkedAccountList.Add(entry);
                }
            }
        }
        public List<int> GetNewlyAddedAccounts()
        {
            List<int> addedAccountList = new List<int>();
            foreach (int entry in LinkedAccountList)
                if (!initialLinkedAccountList.Contains(entry))
                    addedAccountList.Add(entry);
            return addedAccountList;
        }
        public List<int> GetNewlyDeletedAccounts()
        {
            List<int> deletedAccountList = new List<int>();
            foreach (int entry in initialLinkedAccountList)
                if (!LinkedAccountList.Contains(entry))
                    deletedAccountList.Add(entry);
            return deletedAccountList;
        }
        public List<int> GetUnmodifiedAccounts()
        {
            List<int> unmodifiedAccountList = new List<int>();
            foreach (int entry in initialLinkedAccountList)
                if (LinkedAccountList.Contains(entry))
                    unmodifiedAccountList.Add(entry);
            return unmodifiedAccountList;
        }
    }
    public enum PeriodMethod { EveryXUnits, SameXofUnit }
    public enum PeriodType { Day, Week, Month, Quarter, Year }
}