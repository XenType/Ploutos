using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PloutosMain.DataLayer;
using PloutosMain.Models;
using PloutosMain.Repositories;

namespace PloutosMain.Tests.Repositories
{
    [TestClass]
    public class AccountRepoTests
    {
        #region Test Mock Objects
        private CashAssetAccount expectedCashAssetAccount;
        private CreditAssetAccount expectedCreditAssetAccount;
        private SavingsAssetAccount expectedSavingsAssetAccount;

        #endregion

        #region Test Result Constants

        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            expectedCashAssetAccount = CreateExpectedCashAssetAccount();
            expectedCreditAssetAccount = CreateExpectedCreditAssetAccount();
        }

        #endregion

        #region Account Retrieval Tests
        // DataLayer Command Usage
        [TestMethod]
        public void WhenGettingCashAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequest();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object);
            CashAssetAccount actualAccount = (CashAssetAccount)accountRepo.GetAccount(expectedCashAssetAccount.Id);
            /// Expectations
            /// A single call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCashAssetAccount.Id)), Times.Once);
        }
        [TestMethod]
        public void WhenGettingCreditAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequest();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object);
            CreditAssetAccount actualAccount = (CreditAssetAccount)accountRepo.GetAccount(expectedCreditAssetAccount.Id);
            /// Expectations
            /// A single call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCreditAssetAccount.Id)), Times.Once);
        }
        //not ready for this test
        /*
        [TestMethod]
        public void WhenGettingSavingsAssetAccount_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequest();
            IAccountRepo accountRepo = new AccountRepo(mockDataLayer.Object);
            AssetAccount actualAccount = (AssetAccount)accountRepo.GetAccount(SavingsAssetAccountId);
            /// Expectations
            /// A call will be made to retrieve the asset account data
            /// A call will be made to retrieve the time period data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == SavingsAssetAccountId)), Times.Once);
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<int>(y => y == SavingsTimePeriodId)), Times.Once);
        }
        */

        // DataTable Mapping
        [TestMethod]
        public void WhenGettingCashAssetAccount_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequest();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object);
            AssetAccount actualAccount = (AssetAccount)accountRepo.GetAccount(expectedCashAssetAccount.Id);

            Assert.AreEqual(actualAccount.Id, expectedCashAssetAccount.Id);
            Assert.AreEqual(actualAccount.Name, expectedCashAssetAccount.Name);
            Assert.AreEqual(actualAccount.AccountType, expectedCashAssetAccount.AccountType);
            Assert.AreEqual(actualAccount.AssetAccountType, expectedCashAssetAccount.AssetAccountType);
            Assert.AreEqual(actualAccount.Balance, expectedCashAssetAccount.Balance);
        }
        [TestMethod]
        public void WhenGettingCreditAssetAccount_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequest();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object);
            AssetAccount actualAccount = (AssetAccount)accountRepo.GetAccount(expectedCreditAssetAccount.Id);

            Assert.AreEqual(actualAccount.Id, expectedCreditAssetAccount.Id);
            Assert.AreEqual(actualAccount.Name, expectedCreditAssetAccount.Name);
            Assert.AreEqual(actualAccount.AccountType, expectedCreditAssetAccount.AccountType);
            Assert.AreEqual(actualAccount.AssetAccountType, expectedCreditAssetAccount.AssetAccountType);
            Assert.AreEqual(actualAccount.Balance, expectedCreditAssetAccount.Balance);
        }

        // Exception Testings
        // TODO: AssetAccountDataNotFoundException
        #endregion



        #region Moq Helper Methods
        private Mock<IDataLayer> MockAssetAccountRequest()
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
            return dataLayer;
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
            DataTable cashAssetTable = CreateMockAssetDataTable();
            DataRow newRow = cashAssetTable.NewRow();
            newRow[DataObjects.Accounts.Columns.Id] = expectedCreditAssetAccount.Id;
            newRow[DataObjects.Accounts.Columns.Name] = expectedCreditAssetAccount.Name;
            newRow[DataObjects.Accounts.Columns.AccountType] = expectedCreditAssetAccount.AccountType;
            newRow[DataObjects.Accounts.Columns.AssetAccountType] = expectedCreditAssetAccount.AssetAccountType;
            newRow[DataObjects.Accounts.Columns.Balance] = expectedCreditAssetAccount.Balance;
            newRow[DataObjects.Accounts.Columns.LinkedExpenceAccountId] = expectedCreditAssetAccount.LinkedExpenseAccountId;
            cashAssetTable.Rows.Add(newRow);
            return cashAssetTable;
        }
        private DataTable CreateMockAssetDataTable()
        {
            DataTable assetTable = new DataTable();
            DataColumn newColumn = new DataColumn("Id", typeof(int));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn("Name", typeof(string));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn("Type", typeof(int));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn("SubType", typeof(int));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn("Balance", typeof(decimal));
            assetTable.Columns.Add(newColumn);
            newColumn = new DataColumn("LinkId", typeof(int));
            assetTable.Columns.Add(newColumn);
            return assetTable;
        }
        #endregion

        #region Test Helper Methods
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
            newAccount.LinkedExpenseAccountId = 10;
            return newAccount;
        }
        #endregion
    }
}
