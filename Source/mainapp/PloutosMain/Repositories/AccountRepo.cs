using PloutosMain.DataLayer;
using PloutosMain.Models;


namespace PloutosMain.Repositories
{
    public class AccountRepo : IAccountRepo
    {
        private IDataLayer _dataLayer;

        public AccountRepo(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }

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

        }
    }
}