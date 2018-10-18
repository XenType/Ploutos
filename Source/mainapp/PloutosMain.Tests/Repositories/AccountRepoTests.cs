﻿using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PloutosMain.DataLayer;
using PloutosMain.Exceptions;
using PloutosMain.Models;
using PloutosMain.Repositories;

namespace PloutosMain.Tests.Repositories
{
    [TestClass]
    public class AccountRepoTests
    {
        #region Test Mock Objects
        private AssetAccount expectedNotFoundAssetAccount;
        private CashAssetAccount expectedCashAssetAccount;
        private CreditAssetAccount expectedCreditAssetAccount;
        private TimePeriod expectedOwnedCreditTimePeriod;
        private SavingsAssetAccount expectedSavingsAssetAccount;
        private TimePeriod expectedOwnedSavingsTimePeriod;

        private AssetAccount expectedDeleteAssetAccount;

        private AccountNotFoundException expectedAccountNotFoundException;
        #endregion

        #region Test Constants
        private const int expectedNumberOfAccountToTimePeriodLinkGetValues = 1;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            expectedNotFoundAssetAccount = CreateExpectedNotFoundAssetAccount();
            expectedCashAssetAccount = CreateExpectedCashAssetAccount();
            expectedCreditAssetAccount = CreateExpectedCreditAssetAccount();
            expectedOwnedCreditTimePeriod = CreateExpectedOwnedCreditTimePeriod();
            expectedSavingsAssetAccount = CreateExpectedSavingsAssetAccount();
            expectedOwnedSavingsTimePeriod = CreateExpectedOwnedSavingsTimePeriod();

            expectedDeleteAssetAccount = CreateExpectedDeleteAssetAccount();

            expectedAccountNotFoundException = new AccountNotFoundException(expectedNotFoundAssetAccount.Id, AccountType.Asset);
        }

        #endregion

        #region Exception Tests        
        [TestMethod]
        public void WhenAccountDoesNotExist_CorrectExceptionIsThrown()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object);
            Exception actualException = new Exception();
            try
            {
                SavingsAssetAccount actualAccount = (SavingsAssetAccount)accountRepo.GetAccount(expectedNotFoundAssetAccount.Id);
            }
            catch (Exception e)
            {
                actualException = e;
            }
            Assert.AreEqual(expectedAccountNotFoundException.Message, actualException.Message);
        }
        #endregion

        #region Asset Account Retrieval Tests
        // DataLayer Command Usage
        [TestMethod]
        public void WhenGettingCashAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CashAssetAccount actualAccount = (CashAssetAccount)accountRepo.GetAccount(expectedCashAssetAccount.Id);
            /// Expectations
            /// A call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCashAssetAccount.Id)), Times.Once);
        }
        [TestMethod]
        public void WhenGettingCreditAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount actualAccount = (CreditAssetAccount)accountRepo.GetAccount(expectedCreditAssetAccount.Id);
            /// Expectations
            /// A call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCreditAssetAccount.Id)), Times.Once);
            /// A call will be made to retrive the TimePeriod owned by this asset account
            mockDataLayer.Verify(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfAccountToTimePeriodLinkGetValues &&
                    (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedCreditAssetAccount.Id)), 
                Times.Once);
            //TODO: Update when ExpenseAccount is implemented
        }

        [TestMethod]
        public void WhenGettingSavingsAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            AssetAccount actualAccount = (SavingsAssetAccount)accountRepo.GetAccount(expectedSavingsAssetAccount.Id);
            /// Expectations
            /// A call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedSavingsAssetAccount.Id)), Times.Once);
            /// A call will be made to retrive the TimePeriod owned by this asset account
            mockDataLayer.Verify(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfAccountToTimePeriodLinkGetValues &&
                    (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedSavingsAssetAccount.Id)),
                Times.Once);
        }

        // TimePeriodRepo Command Usage
        [TestMethod]
        public void WhenGettingCreditAssetAccount_CorrectTimePeriodRepoCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount actualAccount = (CreditAssetAccount)accountRepo.GetAccount(expectedCreditAssetAccount.Id);
            /// Expectations
            /// A call will be made to retrieve TimePeriod object
            mockTimePeriodRepo.Verify(x => x.GetTimePeriod(
                It.Is<int>(y => y == expectedOwnedCreditTimePeriod.Id)), Times.Once);
        }
        [TestMethod]
        public void WhenGettingSavingsAssetAccount_CorrectTimePeriodRepoCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount actualAccount = (SavingsAssetAccount)accountRepo.GetAccount(expectedSavingsAssetAccount.Id);
            /// Expectations
            /// A call will be made to retrieve TimePeriod object
            mockTimePeriodRepo.Verify(x => x.GetTimePeriod(
                It.Is<int>(y => y == expectedOwnedSavingsTimePeriod.Id)), Times.Once);
        }

        // DataTable Mapping
        [TestMethod]
        public void WhenGettingCashAssetAccount_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CashAssetAccount actualAccount = (CashAssetAccount)accountRepo.GetAccount(expectedCashAssetAccount.Id);

            Assert.AreEqual(expectedCashAssetAccount.Id, actualAccount.Id);
            Assert.AreEqual(expectedCashAssetAccount.Name, actualAccount.Name);
            Assert.AreEqual(expectedCashAssetAccount.AccountType, actualAccount.AccountType);
            Assert.AreEqual(expectedCashAssetAccount.AssetAccountType, actualAccount.AssetAccountType);
            Assert.AreEqual(expectedCashAssetAccount.Balance, actualAccount.Balance);
        }
        [TestMethod]
        public void WhenGettingCreditAssetAccount_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount actualAccount = (CreditAssetAccount)accountRepo.GetAccount(expectedCreditAssetAccount.Id);

            Assert.AreEqual(expectedCreditAssetAccount.Id, actualAccount.Id);
            Assert.AreEqual(expectedCreditAssetAccount.Name, actualAccount.Name);
            Assert.AreEqual(expectedCreditAssetAccount.AccountType, actualAccount.AccountType);
            Assert.AreEqual(expectedCreditAssetAccount.AssetAccountType, actualAccount.AssetAccountType);
            Assert.AreEqual(expectedCreditAssetAccount.Balance, actualAccount.Balance);
            Assert.AreEqual(expectedCreditAssetAccount.CreditLine, actualAccount.CreditLine);
            Assert.AreEqual(expectedCreditAssetAccount.InterestRate, actualAccount.InterestRate);
            //TODO: Update when TimePeriod is implemented
            //TODO: Update when ExpenseAccount is implemented
        }
        [TestMethod]
        public void WhenGettingSavingsAssetAccount_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount actualAccount = (SavingsAssetAccount)accountRepo.GetAccount(expectedSavingsAssetAccount.Id);

            Assert.AreEqual(expectedSavingsAssetAccount.Id, actualAccount.Id);
            Assert.AreEqual(expectedSavingsAssetAccount.Name, actualAccount.Name);
            Assert.AreEqual(expectedSavingsAssetAccount.AccountType, actualAccount.AccountType);
            Assert.AreEqual(expectedSavingsAssetAccount.AssetAccountType, actualAccount.AssetAccountType);
            Assert.AreEqual(expectedSavingsAssetAccount.Balance, actualAccount.Balance);
            Assert.AreEqual(expectedSavingsAssetAccount.InterestRate, actualAccount.InterestRate);
            //TODO: Update when TimePeriod is implemented
        }
        #endregion

        #region Asset Account Creation Tests

        #endregion

        #region Asset Account Editing Tests

        #endregion
        
        #region Asset Account Removal Tests
        [TestMethod]
        public void WhenDeletingAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = new Mock<IDataLayer>();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object);
            accountRepo.DeleteAccount(expectedDeleteAssetAccount.Id);
            /// Expectations
            /// A call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.DeleteRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedDeleteAssetAccount.Id)), Times.Once);
        }
        #endregion

        #region Moq Helper Methods
        private Mock<IDataLayer> MockAssetAccountRequests()
        {
            Mock<IDataLayer> dataLayer = new Mock<IDataLayer>();
            // Cash Asset DataTable
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCashAssetAccount.Id))).Returns(CreateMockCashAssetAccountDataTable());
            // Credit Asset DataTable
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCreditAssetAccount.Id))).Returns(CreateMockCreditAssetAccountDataTable());
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfAccountToTimePeriodLinkGetValues &&
                    (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedCreditAssetAccount.Id)))
                .Returns(CreateMockExampleTimePeriodDataTable(expectedOwnedCreditTimePeriod));
            // Savings Asset DataTable
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedSavingsAssetAccount.Id))).Returns(CreateMockSavingsAssetAccountDataTable());
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfAccountToTimePeriodLinkGetValues &&
                    (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedSavingsAssetAccount.Id)))
                .Returns(CreateMockExampleTimePeriodDataTable(expectedOwnedSavingsTimePeriod));
            return dataLayer;
        }
        private Mock<ITimePeriodRepo> MockTimePeriodRepoRequests()
        {
            return new Mock<ITimePeriodRepo>();
        }
        private DataTable CreateMockCashAssetAccountDataTable()
        {
            DataTable cashAssetTable = CreateMockAssetDataTable();
            DataRow newRow = cashAssetTable.NewRow();
            newRow[DataObjects.Accounts.Columns.Id] = expectedCashAssetAccount.Id;
            newRow[DataObjects.Accounts.Columns.Name] = expectedCashAssetAccount.Name;
            newRow[DataObjects.Accounts.Columns.AccountType] = expectedCashAssetAccount.AccountType;
            newRow[DataObjects.Accounts.Columns.AssetAccountType] = expectedCashAssetAccount.AssetAccountType;
            newRow[DataObjects.Accounts.Columns.Balance] = expectedCashAssetAccount.Balance;
            cashAssetTable.Rows.Add(newRow);
            return cashAssetTable;
        }
        private DataTable CreateMockCreditAssetAccountDataTable()
        {
            DataTable creditAssetTable = CreateMockAssetDataTable();
            DataRow newRow = creditAssetTable.NewRow();
            newRow[DataObjects.Accounts.Columns.Id] = expectedCreditAssetAccount.Id;
            newRow[DataObjects.Accounts.Columns.Name] = expectedCreditAssetAccount.Name;
            newRow[DataObjects.Accounts.Columns.AccountType] = expectedCreditAssetAccount.AccountType;
            newRow[DataObjects.Accounts.Columns.AssetAccountType] = expectedCreditAssetAccount.AssetAccountType;
            newRow[DataObjects.Accounts.Columns.Balance] = expectedCreditAssetAccount.Balance;
            newRow[DataObjects.Accounts.Columns.CreditLine] = expectedCreditAssetAccount.CreditLine;
            newRow[DataObjects.Accounts.Columns.InterestRate] = expectedCreditAssetAccount.InterestRate;
            newRow[DataObjects.Accounts.Columns.StatementTimePeriodId] = expectedCreditAssetAccount.StatementTimePeriod.Id;
            creditAssetTable.Rows.Add(newRow);
            return creditAssetTable;
        }
        private DataTable CreateMockSavingsAssetAccountDataTable()
        {
            DataTable savingsAssetTable = CreateMockAssetDataTable();
            DataRow newRow = savingsAssetTable.NewRow();
            newRow[DataObjects.Accounts.Columns.Id] = expectedSavingsAssetAccount.Id;
            newRow[DataObjects.Accounts.Columns.Name] = expectedSavingsAssetAccount.Name;
            newRow[DataObjects.Accounts.Columns.AccountType] = expectedSavingsAssetAccount.AccountType;
            newRow[DataObjects.Accounts.Columns.AssetAccountType] = expectedSavingsAssetAccount.AssetAccountType;
            newRow[DataObjects.Accounts.Columns.Balance] = expectedSavingsAssetAccount.Balance;
            newRow[DataObjects.Accounts.Columns.InterestRate] = expectedSavingsAssetAccount.InterestRate;
            newRow[DataObjects.Accounts.Columns.StatementTimePeriodId] = expectedSavingsAssetAccount.StatementTimePeriod.Id;
            savingsAssetTable.Rows.Add(newRow);
            return savingsAssetTable;
        }
        private DataTable CreateMockAssetDataTable()
        {
            DataTable assetTable = new DataTable();
            DataColumn newColumn = new DataColumn(DataObjects.Accounts.Columns.Id, typeof(int));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.Name, typeof(string));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.AccountType, typeof(int));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.AssetAccountType, typeof(int));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.Balance, typeof(decimal));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.CreditLine, typeof(decimal));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.InterestRate, typeof(decimal));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.Accounts.Columns.StatementTimePeriodId, typeof(int));
            assetTable.Columns.Add(newColumn);
            return assetTable;
        }
        private DataTable CreateMockExampleTimePeriodDataTable(TimePeriod baseTimePeriod)
        {
            DataTable exampleTimePeriodTable = CreateMockTimePeriodDataTable();
            DataRow newRow = exampleTimePeriodTable.NewRow();
            newRow[DataObjects.TimePeriods.Columns.Id] = baseTimePeriod.Id;
            newRow[DataObjects.TimePeriods.Columns.Name] = baseTimePeriod.Name;
            newRow[DataObjects.TimePeriods.Columns.LastOccurance] = baseTimePeriod.LastOccurance;
            newRow[DataObjects.TimePeriods.Columns.PeriodMethod] = baseTimePeriod.PeriodMethod;
            newRow[DataObjects.TimePeriods.Columns.PeriodType] = baseTimePeriod.PeriodType;
            newRow[DataObjects.TimePeriods.Columns.PeriodValue] = baseTimePeriod.PeriodValue;
            newRow[DataObjects.TimePeriods.Columns.OwnerAccountId] = baseTimePeriod.OwnerAccountId;
            exampleTimePeriodTable.Rows.Add(newRow);
            return exampleTimePeriodTable;
        }
        private DataTable CreateMockTimePeriodDataTable()
        {
            DataTable timePeriodTable = new DataTable();
            DataColumn newColumn = new DataColumn(DataObjects.TimePeriods.Columns.Id, typeof(int));
            timePeriodTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.TimePeriods.Columns.Name, typeof(string));
            timePeriodTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.TimePeriods.Columns.LastOccurance, typeof(DateTime));
            timePeriodTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.TimePeriods.Columns.PeriodMethod, typeof(int));
            timePeriodTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.TimePeriods.Columns.PeriodType, typeof(int));
            timePeriodTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.TimePeriods.Columns.PeriodValue, typeof(int));
            timePeriodTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.TimePeriods.Columns.OwnerAccountId, typeof(int));
            timePeriodTable.Columns.Add(newColumn);
            return timePeriodTable;
        }
        #endregion

        #region Test Helper Methods
        private AssetAccount CreateExpectedNotFoundAssetAccount()
        {
            AssetAccount newAccount = new AssetAccount();
            newAccount.Id = 0;
            return newAccount;
        }
        private CashAssetAccount CreateExpectedCashAssetAccount()
        {
            CashAssetAccount newAccount = new CashAssetAccount();
            newAccount.Id = 1;
            newAccount.Name = "Cash Account";
            newAccount.AccountType = AccountType.Asset;
            newAccount.AssetAccountType = AssetAccountType.Cash;
            newAccount.Balance = 100;
            return newAccount;
        }
        private CreditAssetAccount CreateExpectedCreditAssetAccount()
        {
            CreditAssetAccount newAccount = new CreditAssetAccount();
            newAccount.Id = 2;
            newAccount.Name = "Credit Account";
            newAccount.AccountType = AccountType.Asset;
            newAccount.AssetAccountType = AssetAccountType.Credit;
            newAccount.Balance = 500;
            newAccount.CreditLine = 1000;
            newAccount.InterestRate = .045M;
            //TODO: Update when TimePeriod is implemented
            newAccount.StatementTimePeriod = new TimePeriod()
            {
                Id = 10,
                LastOccurance = DateTime.Parse("1/1/2019"),
                Name = "Credit Account Statement Date",
                OwnerAccountId = 2
            };
            return newAccount;
        }
        private SavingsAssetAccount CreateExpectedSavingsAssetAccount()
        {
            SavingsAssetAccount newAccount = new SavingsAssetAccount();
            newAccount.Id = 3;
            newAccount.Name = "Savings Account";
            newAccount.AccountType = AccountType.Asset;
            newAccount.AssetAccountType = AssetAccountType.Savings;
            newAccount.Balance = 10000;
            newAccount.InterestRate = .03M;
            //TODO: Update when TimePeriod is implemented
            newAccount.StatementTimePeriod = new TimePeriod()
            {
                Id = 11,
                LastOccurance = DateTime.Parse("2/2/2019"),
                Name = "Savings Account Statement Date",
                OwnerAccountId = 3
            };
            return newAccount;
        }

        private AssetAccount CreateExpectedDeleteAssetAccount()
        {
            AssetAccount newAccount = new AssetAccount();
            newAccount.Id = 13;
            return newAccount;
        }

        private TimePeriod CreateExpectedOwnedCreditTimePeriod()
        {
            TimePeriod newTimePeriod = new TimePeriod();
            newTimePeriod.Id = 11;
            newTimePeriod.Name = "Credit Card 1234 Statement Date";
            newTimePeriod.LastOccurance = DateTime.Parse("1/15/2018");
            newTimePeriod.PeriodMethod = PeriodMethod.SameXofUnit;
            newTimePeriod.PeriodType = PeriodType.Month;
            newTimePeriod.PeriodValue = 1;
            newTimePeriod.OwnerAccountId = 2;
            newTimePeriod.LinkedAccountList.Add(7);
            newTimePeriod.LinkedAccountList.Add(8);
            return newTimePeriod;

        }
        private TimePeriod CreateExpectedOwnedSavingsTimePeriod()
        {
            TimePeriod newTimePeriod = new TimePeriod();
            newTimePeriod.Id = 11;
            newTimePeriod.Name = "Savings Statement Date";
            newTimePeriod.LastOccurance = DateTime.Parse("1/1/2018");
            newTimePeriod.PeriodMethod = PeriodMethod.SameXofUnit;
            newTimePeriod.PeriodType = PeriodType.Month;
            newTimePeriod.PeriodValue = 1;
            newTimePeriod.OwnerAccountId = 3;
            newTimePeriod.LinkedAccountList.Add(7);
            newTimePeriod.LinkedAccountList.Add(8);
            return newTimePeriod;

        }
        #endregion
    }
}
