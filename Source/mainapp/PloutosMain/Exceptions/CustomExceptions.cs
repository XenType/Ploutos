﻿using PloutosMain.Models;
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
}