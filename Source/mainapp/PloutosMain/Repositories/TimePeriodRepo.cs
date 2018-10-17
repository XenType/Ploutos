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
        public TimePeriod InsertTimePeriod(TimePeriod unsavedTimePeriod)
        {
            Dictionary<string, object> valueList = MapTimePeriodToDictionary(unsavedTimePeriod);
            DataTable timePeriodData = _dataLayer.InsertRecord(DataObjects.DbTarget.TimePeriod, valueList);

            if (timePeriodData == null || timePeriodData.Rows.Count < 1)
                throw new TimePeriodFailedToReturnAfterInsertException(unsavedTimePeriod.Name, unsavedTimePeriod.OwnerAccountId);

            TimePeriod savedTimePeriod = MapBasicTimePeriod(timePeriodData.Rows[0]);

            PersistAccountToTimePeriodLinks(ref savedTimePeriod, ref unsavedTimePeriod);

            timePeriodData.Dispose();
            
            return savedTimePeriod;
        }
        public TimePeriod UpdateTimePeriod(TimePeriod modifiedTimePeriod)
        {
            return null;
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
        private void PersistAccountToTimePeriodLinks(ref TimePeriod savedTimePeriod, ref TimePeriod unsavedTimePeriod)
        {
            List<int> addedAccountList = unsavedTimePeriod.GetNewlyAddedAccounts();
            Dictionary<string, object> valueList = new Dictionary<string, object>();
            valueList.Add(DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId, savedTimePeriod.Id);
            valueList.Add(DataObjects.AccountToTimePeriodLink.Columns.AccountId, 0);
            DataTable linkedAccountData = new DataTable();
            foreach (int entry in addedAccountList)
            {
                valueList[DataObjects.AccountToTimePeriodLink.Columns.AccountId] = entry;
                if (linkedAccountData == null || linkedAccountData.Rows.Count == 0)
                    linkedAccountData = _dataLayer.InsertRecord(DataObjects.DbTarget.AccountToTimePeriodLink, valueList);
                else
                {
                    DataTable dt = _dataLayer.InsertRecord(DataObjects.DbTarget.AccountToTimePeriodLink, valueList);
                    DataRow newRow = linkedAccountData.NewRow();
                    newRow[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] = dt.Rows[0][DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId];
                    newRow[DataObjects.AccountToTimePeriodLink.Columns.AccountId] = dt.Rows[0][DataObjects.AccountToTimePeriodLink.Columns.AccountId];
                    linkedAccountData.Rows.Add(newRow);
                }
            }
            linkedAccountData.Dispose();
            MapLinkedAccounts(ref savedTimePeriod, linkedAccountData);
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
            List<int> accountList = new List<int>();
            foreach (DataRow row in linkedAccountTable.Rows)
            {
                int accountId = int.Parse(row[DataObjects.AccountToTimePeriodLink.Columns.AccountId].ToString());
                if (accountId != timePeriod.OwnerAccountId)
                    accountList.Add(accountId);
            }
            timePeriod.EstablishInitialLinkedAccountList(accountList);
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