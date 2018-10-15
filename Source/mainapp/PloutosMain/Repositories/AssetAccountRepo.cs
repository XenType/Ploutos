using PloutosMain.DataLayer;
using PloutosMain.Exceptions;
using PloutosMain.Models;
using System.Data;

namespace PloutosMain.Repositories
{
    public class AssetAccountRepo : IAccountRepo
    {
        #region private objects
        private IDataLayer _dataLayer;
        private Account _account;
        #endregion

        #region constructors
        public AssetAccountRepo(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
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
                    MapCashAccount(assetAccount, accountData.Rows[0]);
                    break;
                case AssetAccountType.Credit:
                    MapCreditAccount(assetAccount, accountData.Rows[0]);
                    break;
                case AssetAccountType.Savings:
                    MapSavingsAccount(assetAccount, accountData.Rows[0]);
                    break;
            }

            accountData.Dispose();
            return _account;
        }
        public Account InsertAccount(Account newAccount)
        {
            return _account;
        }
        public Account UpdateAccount(Account modifiedAccount)
        {
            return _account;
        }
        public void DeleteAccount(int accountId)
        {
            _dataLayer.DeleteRecord(DataObjects.DbTarget.Account, accountId);
        }
        #endregion

        #region private control methods

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
        private void MapCashAccount(AssetAccount assetAccount, DataRow accountData)
        {
            CashAssetAccount cashAccount = new CashAssetAccount(assetAccount);
            _account = cashAccount;
        }
        private void MapCreditAccount(AssetAccount assetAccount, DataRow accountData)
        {
            CreditAssetAccount creditAccount = new CreditAssetAccount(assetAccount);
            creditAccount.CreditLine = decimal.Parse(accountData[DataObjects.Accounts.Columns.CreditLine].ToString());
            creditAccount.InterestRate = decimal.Parse(accountData[DataObjects.Accounts.Columns.InterestRate].ToString());
            //TODO: Update when TimePeriod is implemented
            _account = creditAccount;
        }
        private void MapSavingsAccount(AssetAccount assetAccount, DataRow accountData)
        {
            SavingsAssetAccount savingsAccount = new SavingsAssetAccount(assetAccount);
            savingsAccount.InterestRate = decimal.Parse(accountData[DataObjects.Accounts.Columns.InterestRate].ToString());
            //TODO: Update when TimePeriod is implemented
            _account = savingsAccount;

        }
        #endregion
    }
}