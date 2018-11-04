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

            AssetAccount assetAccount = BuildAssetAccountFromDataRow(accountData.Rows[0]);
            
            accountData.Dispose();

            return assetAccount;
        }
        public Account InsertAccount(Account newAccount)
        {
            AssetAccount newAssetAccount = (AssetAccount)newAccount;
            Dictionary<string, object> valueList = MapBasicAssetAccountToDictionary(newAssetAccount);
            AssetAccount savedAssetAccount = new AssetAccount();
            DataTable accountData = new DataTable();

            switch (newAssetAccount.AssetAccountType)
            {
                case AssetAccountType.Cash:
                    CashAssetAccount cashAssetAccount = (CashAssetAccount)newAssetAccount;
                    MapCashAssetAccountToDictionary(cashAssetAccount, ref valueList);
                    accountData = _dataLayer.InsertRecord(DataObjects.DbTarget.Account, valueList);                      
                    break;
                case AssetAccountType.Credit:
                    CreditAssetAccount creditAssetAccount = (CreditAssetAccount)newAssetAccount;
                    MapCreditAssetAccountToDictionary(creditAssetAccount, ref valueList);
                    accountData = _dataLayer.InsertRecord(DataObjects.DbTarget.Account, valueList);
                    if (accountData != null && accountData.Rows.Count > 0)
                        InsertOwnedTimePeriod(accountData, creditAssetAccount);
                    break;
                case AssetAccountType.Savings:
                    SavingsAssetAccount savingsAssetAccount = (SavingsAssetAccount)newAssetAccount;
                    MapSavingsAssetAccountToDictionary(savingsAssetAccount, ref valueList);
                    accountData = _dataLayer.InsertRecord(DataObjects.DbTarget.Account, valueList);
                    if (accountData != null && accountData.Rows.Count > 0)
                        InsertOwnedTimePeriod(accountData, null, savingsAssetAccount);
                    break;
            }

            if (accountData != null && accountData.Rows.Count > 0)
                savedAssetAccount = BuildAssetAccountFromDataRow(accountData.Rows[0]);

            return savedAssetAccount;
        }
        public Account UpdateAccount(Account modifiedAccount)
        {
            AssetAccount modifiedAssetAccount = (AssetAccount)modifiedAccount;
            Dictionary<string, object> valueList = MapBasicAssetAccountToDictionary(modifiedAssetAccount, true);
            AssetAccount savedAssetAccount = new AssetAccount();
            DataTable accountData = new DataTable();
            
            switch (modifiedAssetAccount.AssetAccountType)
            {
                case AssetAccountType.Cash:
                    accountData = _dataLayer.UpdateRecord(DataObjects.DbTarget.Account, valueList, modifiedAccount.Id);
                    break;
                case AssetAccountType.Credit:
                    CreditAssetAccount creditAssetAccount = (CreditAssetAccount)modifiedAssetAccount;
                    MapCreditAssetAccountToDictionary(creditAssetAccount, ref valueList, true);
                    accountData = _dataLayer.UpdateRecord(DataObjects.DbTarget.Account, valueList, modifiedAccount.Id);
                    break;
                case AssetAccountType.Savings:
                    SavingsAssetAccount savingsAssetAccount = (SavingsAssetAccount)modifiedAssetAccount;
                    MapSavingsAssetAccountToDictionary(savingsAssetAccount, ref valueList, true);
                    accountData = _dataLayer.UpdateRecord(DataObjects.DbTarget.Account, valueList, modifiedAccount.Id);
                    break;
            }

            if (accountData != null && accountData.Rows.Count > 0)
                savedAssetAccount = BuildAssetAccountFromDataRow(accountData.Rows[0]);

            return savedAssetAccount;
        }
        public void DeleteAccount(Account oldAccount)
        {
            AssetAccount oldAssetAccount = (AssetAccount)oldAccount;

            if (oldAssetAccount.AssetAccountType != AssetAccountType.Cash)
                _dataLayer.DeleteRecords(DataObjects.DbTarget.AccountToTimePeriodLink, MapAccountIdToDictionary(oldAssetAccount.Id));

            if (oldAssetAccount.AssetAccountType == AssetAccountType.Credit)
            {
                CreditAssetAccount creditAssetAccount = (CreditAssetAccount)oldAssetAccount;
                if (creditAssetAccount.StatementTimePeriod != null)
                    _timePeriodRepo.DeleteTimePeriod(creditAssetAccount.StatementTimePeriod.Id);
            }
            if (oldAssetAccount.AssetAccountType == AssetAccountType.Savings)
            {
                SavingsAssetAccount savingsAssetAccount = (SavingsAssetAccount)oldAssetAccount;
                if (savingsAssetAccount.StatementTimePeriod != null)
                    _timePeriodRepo.DeleteTimePeriod(savingsAssetAccount.StatementTimePeriod.Id);
            }
            _dataLayer.DeleteRecord(DataObjects.DbTarget.Account, oldAssetAccount.Id);

        }
        #endregion

        #region private shared methods
        private AssetAccount BuildAssetAccountFromDataRow(DataRow accountData)
        {
            AssetAccount assetAccount = MapBasicAssetAccount(accountData);
            switch (assetAccount.AssetAccountType)
            {
                case AssetAccountType.Cash:
                    assetAccount = new CashAssetAccount(assetAccount);
                    break;
                case AssetAccountType.Credit:
                    assetAccount = MapCreditAccount(assetAccount, accountData);
                    break;
                case AssetAccountType.Savings:
                    assetAccount = MapSavingsAccount(assetAccount, accountData);
                    break;
            }
            return assetAccount;
        }
        private TimePeriod GetOwnedTimePeriod(AssetAccount assetAccount)
        {
            DataTable timePeriodData = _dataLayer.GetRecords(DataObjects.DbTarget.TimePeriod, MapTimePeriodOwnerToDictionary(assetAccount));
            if (timePeriodData == null || timePeriodData.Rows.Count == 0)
                return null;
            return _timePeriodRepo.GetTimePeriod(int.Parse(timePeriodData.Rows[0][DataObjects.TimePeriods.Columns.Id].ToString()));
        }
        private void InsertOwnedTimePeriod(DataTable accountData, CreditAssetAccount newCreditAccount = null, SavingsAssetAccount newSavingsAccount = null)
        {
            if (newCreditAccount != null && newCreditAccount.StatementTimePeriod != null)
            {
                newCreditAccount.StatementTimePeriod.OwnerAccountId = int.Parse(accountData.Rows[0][DataObjects.Accounts.Columns.Id].ToString());
                _timePeriodRepo.InsertTimePeriod(newCreditAccount.StatementTimePeriod);
            }
            else if (newSavingsAccount != null && newSavingsAccount.StatementTimePeriod != null)
            {
                newSavingsAccount.StatementTimePeriod.OwnerAccountId = int.Parse(accountData.Rows[0][DataObjects.Accounts.Columns.Id].ToString());
                _timePeriodRepo.InsertTimePeriod(newSavingsAccount.StatementTimePeriod);
            }
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
        private Dictionary<string, object> MapAccountIdToDictionary(int accountId)
        {
            Dictionary<string, object> criteriaList = new Dictionary<string, object>();
            criteriaList.Add(DataObjects.AccountToTimePeriodLink.Columns.AccountId, accountId);
            return criteriaList;
        }
        private Dictionary<string, object> MapTimePeriodOwnerToDictionary(AssetAccount assetAccount)
        {
            Dictionary<string, object> criteriaList = new Dictionary<string, object>();
            criteriaList.Add(DataObjects.TimePeriods.Columns.OwnerAccountId, assetAccount.Id);
            return criteriaList;
        }
        private Dictionary<string, object> MapBasicAssetAccountToDictionary(AssetAccount assetAccount, bool forUpdate = false)
        {
            Dictionary<string, object> valueList = new Dictionary<string, object>();
            valueList.Add(DataObjects.Accounts.Columns.Name, assetAccount.Name);
            if (!forUpdate) valueList.Add(DataObjects.Accounts.Columns.AccountType, AccountType.Asset);
            valueList.Add(DataObjects.Accounts.Columns.Balance, assetAccount.Balance);
            return valueList;
        }
        private void MapCashAssetAccountToDictionary(CashAssetAccount assetAccount, ref Dictionary<string, object> valueList)
        {
            valueList.Add(DataObjects.Accounts.Columns.AssetAccountType, AssetAccountType.Cash);
        }
        private void MapCreditAssetAccountToDictionary(CreditAssetAccount assetAccount, ref Dictionary<string, object> valueList, bool forUpdate = false)
        {
            valueList.Add(DataObjects.Accounts.Columns.CreditLine, assetAccount.CreditLine);
            valueList.Add(DataObjects.Accounts.Columns.InterestRate, assetAccount.InterestRate);
            if (!forUpdate) valueList.Add(DataObjects.Accounts.Columns.AssetAccountType, AssetAccountType.Credit);
        }
        private void MapSavingsAssetAccountToDictionary(SavingsAssetAccount assetAccount, ref Dictionary<string, object> valueList, bool forUpdate = false)
        {
            valueList.Add(DataObjects.Accounts.Columns.InterestRate, assetAccount.InterestRate);
            if (!forUpdate) valueList.Add(DataObjects.Accounts.Columns.AssetAccountType, AssetAccountType.Savings);
        }
        #endregion
    }
}