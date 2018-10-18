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
            timePeriodData.Dispose();

            AddLinkedAccountList(ref timePeriod);
            
            return timePeriod;
        }
        public TimePeriod InsertTimePeriod(TimePeriod unsavedTimePeriod)
        {
            Dictionary<string, object> valueList = MapTimePeriodToDictionary(unsavedTimePeriod);
            DataTable timePeriodData = _dataLayer.InsertRecord(DataObjects.DbTarget.TimePeriod, valueList);

            if (timePeriodData == null || timePeriodData.Rows.Count < 1)
                throw new TimePeriodFailedToReturnAfterInsertException(unsavedTimePeriod.Name, unsavedTimePeriod.OwnerAccountId);

            TimePeriod savedTimePeriod = MapBasicTimePeriod(timePeriodData.Rows[0]);

            timePeriodData.Dispose();
            
            return savedTimePeriod;
        }
        public TimePeriod UpdateTimePeriod(TimePeriod modifiedTimePeriod)
        {
            Dictionary<string, object> valueList = MapTimePeriodToDictionary(modifiedTimePeriod);
            DataTable timePeriodData = _dataLayer.UpdateRecord(DataObjects.DbTarget.TimePeriod, valueList, modifiedTimePeriod.Id);

            if (timePeriodData == null || timePeriodData.Rows.Count < 1)
                throw new TimePeriodNotFoundException(modifiedTimePeriod.Id);

            TimePeriod savedTimePeriod = MapBasicTimePeriod(timePeriodData.Rows[0]);
            timePeriodData.Dispose();

            AddLinkedAccountList(ref savedTimePeriod);

            return savedTimePeriod;
        }
        public void DeleteTimePeriod(int timePeriodId)
        {
            Dictionary<string, object> criteriaList = new Dictionary<string, object>();
            criteriaList.Add(DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId, timePeriodId);
            _dataLayer.DeleteRecords(DataObjects.DbTarget.AccountToTimePeriodLink, criteriaList);

            _dataLayer.DeleteRecord(DataObjects.DbTarget.TimePeriod, timePeriodId);
        }
        #endregion

        #region private shared methods
        private void AddLinkedAccountList(ref TimePeriod timePeriod)
        {
            Dictionary<string, object> criteriaList = new Dictionary<string, object>();
            criteriaList.Add(DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId, timePeriod.Id);
            DataTable linkedAccountData = _dataLayer.GetRecords(DataObjects.DbTarget.AccountToTimePeriodLink, criteriaList);

            MapLinkedAccounts(ref timePeriod, linkedAccountData);
            linkedAccountData.Dispose();
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
        private Dictionary<string, object> MapTimePeriodToDictionary(TimePeriod newTimePeriod)
        {
            Dictionary<string, object> valueList = new Dictionary<string, object>();
            valueList.Add(DataObjects.TimePeriods.Columns.Name, newTimePeriod.Name);
            valueList.Add(DataObjects.TimePeriods.Columns.LastOccurance, newTimePeriod.LastOccurance);
            valueList.Add(DataObjects.TimePeriods.Columns.PeriodMethod, newTimePeriod.PeriodMethod);
            valueList.Add(DataObjects.TimePeriods.Columns.PeriodType, newTimePeriod.PeriodType);
            valueList.Add(DataObjects.TimePeriods.Columns.PeriodValue, newTimePeriod.PeriodValue);
            valueList.Add(DataObjects.TimePeriods.Columns.OwnerAccountId, newTimePeriod.OwnerAccountId);
            return valueList;
        }
        #endregion
    }
}