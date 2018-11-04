using PloutosMain.DataLayer;
using PloutosMain.Models;
using System;


namespace PloutosMain.Repositories
{
    public class IncomeAccountRepo : IAccountRepo
    {
        #region private objects
        private IDataLayer _dataLayer;
        private ITimePeriodRepo _timePeriodRepo;
        #endregion

        #region constructors
        public IncomeAccountRepo(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
            _timePeriodRepo = new TimePeriodRepo(_dataLayer);
        }
        #endregion

        #region public methods
        public Account GetAccount(int accountId)
        {
            return null;
        }
        public Account InsertAccount(Account newAccount)
        {
            return null;
        }
        public Account UpdateAccount(Account modifiedAccount)
        {
            return null;
        }
        public void DeleteAccount(int accountId)
        {
            return;
        }
        #endregion

        #region private shared methods

        #endregion

        #region private data mapping methods

        #endregion
    }
}