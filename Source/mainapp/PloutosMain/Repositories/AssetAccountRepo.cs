using PloutosMain.DataLayer;
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
            //exceptoin here
            if (accountData == null)
                return _account;

            AssetAccount assetAccount = CreateBasicAssetAccount(accountData.Rows[0]);
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
            return _account;
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

        }
        #endregion

        #region private control methods

        #endregion

        #region private data mapping methods
        private void MapCashAccount(AssetAccount assetAccount, DataRow accountData)
        {
            CashAssetAccount cashAccount = new CashAssetAccount(assetAccount);
            _account = cashAccount;
        }
        private void MapCreditAccount(AssetAccount assetAccount, DataRow accountData)
        {
            CreditAssetAccount creditAccount = new CreditAssetAccount(assetAccount);
            creditAccount.LinkedExpenseAccountId = int.Parse(accountData[DataObjects.Accounts.Columns.LinkedExpenceAccountId].ToString());
            _account = creditAccount;
        }
        private void MapSavingsAccount(AssetAccount assetAccount, DataRow accountData)
        {

        }
        private AssetAccount CreateBasicAssetAccount(DataRow accountData)
        {
            AssetAccount assetAccount = new AssetAccount();
            assetAccount.Id = int.Parse(accountData[DataObjects.Accounts.Columns.Id].ToString());
            assetAccount.Name = accountData[DataObjects.Accounts.Columns.Name].ToString();
            assetAccount.AccountType = (AccountType)int.Parse(accountData[DataObjects.Accounts.Columns.AccountType].ToString());
            assetAccount.AssetAccountType = (AssetAccountType)int.Parse(accountData[DataObjects.Accounts.Columns.AssetAccountType].ToString());
            assetAccount.Balance = decimal.Parse(accountData[DataObjects.Accounts.Columns.Balance].ToString());
            return assetAccount;
        }
        #endregion
    }
}