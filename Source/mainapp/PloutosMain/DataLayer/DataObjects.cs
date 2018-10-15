namespace PloutosMain.DataLayer
{
    public static class DataObjects
    {
        public enum DbTarget { Account, Category, TimePeriod, AccountToTimePeriodLink }

        public static class Accounts
        {
            public static class Columns
            {
                public const string Id = "Id";
                public const string Name = "Name";
                public const string AccountType = "Type";
                public const string AssetAccountType = "SubType";
                public const string Balance = "Balance";
                public const string CreditLine = "Limit";
                public const string InterestRate = "Rate";
                public const string StatementTimePeriodId = "TimePeriodId";
            }
        }
        public static class TimePeriods
        {
            public static class Columns
            {
                public const string Id = "Id";
                public const string Name = "Name";
                public const string LastOccurance = "LastOccurance";
                public const string PeriodMethod = "PeriodMethod";
                public const string PeriodType = "PeriodType";
                public const string PeriodValue = "PeriodValue";
                public const string OwnerAccountId = "OwnerAccountId";
            }
        }
        public static class AccountToTimePeriodLink
        {
            public static class Columns
            {
                public const string AccountId = "AccountId";
                public const string TimePeriodId = "TimePeriodId";
            }
        }
    }
}