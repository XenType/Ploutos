namespace PloutosMain.DataLayer
{
    public static class DataObjects
    {
        public enum DbTarget { Account, Category, TimePeriod }

        public static class Accounts
        {
            public static class Columns
            {
                public const string Id = "Id";
                public const string Name = "Name";
                public const string AccountType = "Type";
                public const string AssetAccountType = "SubType";
                public const string Balance = "Balance";
                public const string LinkedExpenceAccountId = "LinkId";
            }
        }
    }
}