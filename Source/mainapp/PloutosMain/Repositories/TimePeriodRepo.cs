using PloutosMain.DataLayer;
using PloutosMain.Exceptions;
using PloutosMain.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace PloutosMain.Repositories
{
    public class TimePeriodRepo : ITimePeriodRepo
    {
        #region private objects
        private IDataLayer _dataLayer;
        #endregion

        #region constructors
        public TimePeriodRepo(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }
        #endregion

        #region public methods
        public TimePeriod GetTimePeriod(int timePeriodId)
        {
            DataTable timePeriodData = _dataLayer.GetRecord(DataObjects.DbTarget.TimePeriod, timePeriodId);

            if (timePeriodData == null || timePeriodData.Rows.Count < 1)
                throw new TimePeriodNotFoundException(timePeriodId);

            TimePeriod timePeriod = MapBasicTimePeriod(timePeriodData.Rows[0]);

            Dictionary<string, object> criteriaList = new Dictionary<string, object>();
            criteriaList.Add(DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId, timePeriodId);
            DataTable linkedAccountData = _dataLayer.GetRecords(DataObjects.DbTarget.AccountToTimePeriodLink, criteriaList);

            MapLinkedAccounts(ref timePeriod, linkedAccountData);

            timePeriodData.Dispose();
            linkedAccountData.Dispose();

            return timePeriod;
        }
        public TimePeriod InsertTimePeriod(TimePeriod newTimePeriod)
        {
            return null;
        }
        public TimePeriod UpdateTimePeriod(TimePeriod modifiedTimePeriod)
        {
            return null;
        }
        public void DeleteTimePeriod(int timePeriodId)
        {
            //TODO: needs to delete links as well, at this point we have passed an 'are you sure' check
        }
        #endregion

        #region private data mapping methods
        private TimePeriod MapBasicTimePeriod(DataRow timePeriodData)
        {
            TimePeriod timePeriod = new TimePeriod();
            timePeriod.Id = int.Parse(timePeriodData[DataObjects.TimePeriods.Columns.Id].ToString());
            timePeriod.Name = timePeriodData[DataObjects.TimePeriods.Columns.Name].ToString();
            timePeriod.LastOccurance = DateTime.Parse(timePeriodData[DataObjects.TimePeriods.Columns.LastOccurance].ToString());
            timePeriod.PeriodMethod = (PeriodMethod)int.Parse(timePeriodData[DataObjects.TimePeriods.Columns.PeriodMethod].ToString());
            timePeriod.PeriodType = (PeriodType)int.Parse(timePeriodData[DataObjects.TimePeriods.Columns.PeriodType].ToString());
            timePeriod.PeriodValue = int.Parse(timePeriodData[DataObjects.TimePeriods.Columns.PeriodValue].ToString());
            timePeriod.OwnerAccountId = int.Parse(timePeriodData[DataObjects.TimePeriods.Columns.OwnerAccountId].ToString());
            return timePeriod;
        }
        private void MapLinkedAccounts(ref TimePeriod timePeriod, DataTable linkedAccountTable)
        {
            foreach (DataRow row in linkedAccountTable.Rows)
            {
                int accountId = int.Parse(row[DataObjects.AccountToTimePeriodLink.Columns.AccountId].ToString());
                if (accountId != timePeriod.OwnerAccountId)
                    timePeriod.LinkedAccountList.Add(accountId);
            }
        }
        #endregion
    }
}