using PloutosMain.Models;

namespace PloutosMain.Repositories
{
    public interface IAccountRepo
    {
        Account GetAccount(int accountId);
        Account InsertAccount(Account newAccount);
        Account UpdateAccount(Account modifiedAccount);
        void DeleteAccount(Account oldAccount);

    }
}
