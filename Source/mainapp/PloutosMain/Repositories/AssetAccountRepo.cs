using PloutosMain.DataLayer;
using PloutosMain.Exceptions;
using PloutosMain.Models;
using System.Collections.Generic;
using System.Data;

namespace PloutosMain.Repositories
{
    public class AssetAccountRepo : IAccountRepo
    {
        #region private objects
        private IDataLayer _dataLayer;
        private ITimePeriodRepo _timePeriodRepo;
        private Account _account;
        #endregion

        #region constructors
        public AssetAccountRepo(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
            _timePeriodRepo = new TimePeriodRepo(_dataLayer);
        }
        public AssetAccountRepo(IDataLayer dataLayer, ITimePeriodRepo timePeriodRepo)
        {
            _dataLayer = dataLayer;
            _timePeriodRepo = timePeriodRepo;
        }
        #endregion

        #region public methods
        public Account GetAccount(int accountId)
        {
            DataTable accountData = _dataLayer.GetRecord(DataObjects.DbTarget.Account, accountId);
            
            if (accountData == null || accountData.Rows.Count < 1)
                throw new AccountNotFoundException(accountId, AccountType.Asset);

            AssetAccount assetAccount = MapBasicAssetAccount(accountData.Rows[0]);
            switch (assetAccount.AssetAccountType)
            {
                case AssetAccountType.Cash:
                    assetAccount = new CashAssetAccount(assetAccount);
                    break;
                case AssetAccountType.Credit:
                    assetAccount = MapCreditAccount(assetAccount, accountData.Rows[0]);
                    break;
                case AssetAccountType.Savings:
                    assetAccount = MapSavingsAccount(assetAccount, accountData.Rows[0]);
                    break;
            }
            accountData.Dispose();

            return assetAccount;
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
            //remove associated time period if found
            _dataLayer.DeleteRecord(DataObjects.DbTarget.Account, accountId);
        }
        #endregion

        #region private shared methods
        private TimePeriod GetOwnedTimePeriod(AssetAccount assetAccount)
        {
            DataTable timePeriodData = _dataLayer.GetRecords(DataObjects.DbTarget.TimePeriod, MapTimePeriodOwnerToDictionary(assetAccount));
            if (timePeriodData == null || timePeriodData.Rows.Count == 0)
                return null;
            return _timePeriodRepo.GetTimePeriod(int.Parse(timePeriodData.Rows[0][DataObjects.TimePeriods.Columns.Id].ToString()));
        }
        #endregion

        #region private data mapping methods
        private AssetAccount MapBasicAssetAccount(DataRow accountData)
        {
            AssetAccount assetAccount = new AssetAccount();
            assetAccount.Id = int.Parse(accountData[DataObjects.Accounts.Columns.Id].ToString());
            assetAccount.Name = accountData[DataObjects.Accounts.Columns.Name].ToString();
            assetAccount.AccountType = (AccountType)int.Parse(accountData[DataObjects.Accounts.Columns.AccountType].ToString());
            assetAccount.AssetAccountType = (AssetAccountType)int.Parse(accountData[DataObjects.Accounts.Columns.AssetAccountType].ToString());
            assetAccount.Balance = decimal.Parse(accountData[DataObjects.Accounts.Columns.Balance].ToString());

            return assetAccount;
        }
        private CreditAssetAccount MapCreditAccount(AssetAccount assetAccount, DataRow accountData)
        {
            CreditAssetAccount creditAccount = new CreditAssetAccount(assetAccount);
            creditAccount.CreditLine = decimal.Parse(accountData[DataObjects.Accounts.Columns.CreditLine].ToString());
            creditAccount.InterestRate = decimal.Parse(accountData[DataObjects.Accounts.Columns.InterestRate].ToString());
            TimePeriod ownedTimePeriod = GetOwnedTimePeriod(creditAccount);
            if (ownedTimePeriod != null)
                creditAccount.StatementTimePeriod = ownedTimePeriod;
            //TODO: Update when ExpenseAccount is implemented
            return creditAccount;
        }
        private SavingsAssetAccount MapSavingsAccount(AssetAccount assetAccount, DataRow accountData)
        {
            SavingsAssetAccount savingsAccount = new SavingsAssetAccount(assetAccount);
            savingsAccount.InterestRate = decimal.Parse(accountData[DataObjects.Accounts.Columns.InterestRate].ToString());
            TimePeriod ownedTimePeriod = GetOwnedTimePeriod(savingsAccount);
            if (ownedTimePeriod != null)
                savingsAccount.StatementTimePeriod = ownedTimePeriod;
            return savingsAccount;

        }
        private Dictionary<string, object> MapTimePeriodOwnerToDictionary(AssetAccount assetAccount)
        {
            Dictionary<string, object> criteriaList = new Dictionary<string, object>();
            criteriaList.Add(DataObjects.TimePeriods.Columns.OwnerAccountId, assetAccount.Id);
            return criteriaList;
        }
        #endregion
    }
}