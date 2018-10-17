using PloutosMain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PloutosMain.Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException()
        {
        }

        public AccountNotFoundException(int accountId, AccountType accountType)
            : base($"Account {accountId} of type {accountType} not found.")
        {
        }

        public AccountNotFoundException(int accountId, AccountType accountType, Exception inner)
            : base($"Account {accountId} of type {accountType} not found.", inner)
        {
        }
    }

    public class TimePeriodNotFoundException : Exception
    {
        public TimePeriodNotFoundException()
        {
        }

        public TimePeriodNotFoundException(int timePeriodId)
            : base($"TimePeriod {timePeriodId} not found.")
        {
        }

        public TimePeriodNotFoundException(int timePeriodId, Exception inner)
            : base($"TimePeriod {timePeriodId} not found.", inner)
        {
        }
    }
    public class TimePeriodFailedToReturnAfterInsertException : Exception
    {
        public TimePeriodFailedToReturnAfterInsertException()
        {
        }

        public TimePeriodFailedToReturnAfterInsertException(string name, int accountId)
            : base($"New TimePeriod '{name}' for AccountId {accountId} failed to return after insert.")
        {
        }

        public TimePeriodFailedToReturnAfterInsertException(string name, int accountId, Exception inner)
            : base($"New TimePeriod '{name}' for AccountId {accountId} failed to return after insert.", inner)
        {
        }
    }
}