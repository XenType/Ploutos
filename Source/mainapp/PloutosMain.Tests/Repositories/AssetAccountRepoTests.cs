using System;
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
    public class AssetAccountRepoTests
    {
        #region Test Mock Objects
        private AssetAccount expectedNotFoundAssetAccount;
        private CashAssetAccount expectedCashAssetAccount;
        private CreditAssetAccount expectedCreditAssetAccount;
        private TimePeriod expectedOwnedCreditTimePeriod;
        private SavingsAssetAccount expectedSavingsAssetAccount;
        private TimePeriod expectedOwnedSavingsTimePeriod;
        private AssetAccount expectedDeleteAssetAccount;
        private CreditAssetAccount expectedDeleteCreditAssetAccount;
        private SavingsAssetAccount expectedDeleteSavingsAssetAccount;

        private AccountNotFoundException expectedAccountNotFoundException;
        #endregion

        #region Test Constants
        private const int expectedNumberOfAccountToTimePeriodLinkGetValues = 1;
        private const int expectedNumberOfCashAssetAccountInsertValues = 4;
        private const int expectedNumberOfCashAssetAccountUpdateValues = 2;
        private const int expectedNumberOfCreditAssetAccountInsertValues = 6;
        private const int expectedNumberOfCreditAssetAccountUpdateValues = 4;
        private const int expectedNumberOfSavingsAssetAccountInsertValues = 5;
        private const int expectedNumberOfSavingsAssetAccountUpdateValues = 3;

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
            expectedDeleteCreditAssetAccount = CreateExpectedCreditAssetAccount();
            expectedDeleteSavingsAssetAccount = CreateExpectedSavingsAssetAccount();
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
            Assert.AreEqual(expectedCreditAssetAccount.StatementTimePeriod.Id, actualAccount.StatementTimePeriod.Id);
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
            Assert.AreEqual(expectedSavingsAssetAccount.StatementTimePeriod.Id, actualAccount.StatementTimePeriod.Id);
        }
        #endregion

        #region Asset Account Creation Tests
        [TestMethod]
        public void WhenInsertingCashAssetAccountData_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CashAssetAccount actualCashAssetAccount = (CashAssetAccount)accountRepo.InsertAccount(expectedCashAssetAccount);

            mockDataLayer.Verify(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCashAssetAccountInsertValues && 
                (string)y[DataObjects.Accounts.Columns.Name] == expectedCashAssetAccount.Name &&
                (AccountType)y[DataObjects.Accounts.Columns.AccountType] == expectedCashAssetAccount.AccountType &&
                (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCashAssetAccount.Balance && 
                (AssetAccountType)y[DataObjects.Accounts.Columns.AssetAccountType] == expectedCashAssetAccount.AssetAccountType)),
            Times.Once);
        }
        [TestMethod]
        public void WhenInsertingCashAssetAccountData_CorrectCashAssetAccountObjectIsReturned()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CashAssetAccount actualCashAssetAccount = (CashAssetAccount)accountRepo.InsertAccount(expectedCashAssetAccount);

            Assert.AreEqual(expectedCashAssetAccount.Id, actualCashAssetAccount.Id);
        }
        [TestMethod]
        public void WhenInsertingCreditAssetAccountData_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount actualCreditAssetAccount = (CreditAssetAccount)accountRepo.InsertAccount(expectedCreditAssetAccount);

            mockDataLayer.Verify(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCreditAssetAccountInsertValues &&
                (string)y[DataObjects.Accounts.Columns.Name] == expectedCreditAssetAccount.Name &&
                (AccountType)y[DataObjects.Accounts.Columns.AccountType] == expectedCreditAssetAccount.AccountType &&
                (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCreditAssetAccount.Balance &&
                (AssetAccountType)y[DataObjects.Accounts.Columns.AssetAccountType] == expectedCreditAssetAccount.AssetAccountType &&
                (decimal)y[DataObjects.Accounts.Columns.CreditLine] == expectedCreditAssetAccount.CreditLine &&
                (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedCreditAssetAccount.InterestRate)),
            Times.Once);
        }
        [TestMethod]
        public void WhenInsertingCreditAssetAccountData_CorrectCreditAssetAccountObjectIsReturned()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount newAccount = CreateExpectedCreditAssetAccount();
            newAccount.Id = 0;
            newAccount.StatementTimePeriod.Id = 0;
            newAccount.StatementTimePeriod.OwnerAccountId = 0;
            CreditAssetAccount actualCreditAssetAccount = (CreditAssetAccount)accountRepo.InsertAccount(newAccount);

            Assert.AreEqual(expectedCreditAssetAccount.Id, actualCreditAssetAccount.Id);
            Assert.AreEqual(expectedCreditAssetAccount.StatementTimePeriod.OwnerAccountId, actualCreditAssetAccount.StatementTimePeriod.OwnerAccountId);
        }
        [TestMethod]
        public void WhenInsertingCreditAssetAccountData_CorrectTimePeriodRepoCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount newAccount = CreateExpectedCreditAssetAccount();
            newAccount.Id = 0;
            newAccount.StatementTimePeriod.Id = 0;
            newAccount.StatementTimePeriod.OwnerAccountId = 0;
            CreditAssetAccount actualCreditAssetAccount = (CreditAssetAccount)accountRepo.InsertAccount(newAccount);

            mockTimePeriodRepo.Verify(x => x.InsertTimePeriod(
                It.Is<TimePeriod>(y => 
                    y.Name == expectedCreditAssetAccount.StatementTimePeriod.Name &&
                    y.LastOccurance == expectedCreditAssetAccount.StatementTimePeriod.LastOccurance &&
                    y.PeriodMethod == expectedCreditAssetAccount.StatementTimePeriod.PeriodMethod &&
                    y.PeriodType == expectedCreditAssetAccount.StatementTimePeriod.PeriodType &&
                    y.PeriodValue == expectedCreditAssetAccount.StatementTimePeriod.PeriodValue &&
                    y.OwnerAccountId == expectedCreditAssetAccount.Id)),
                Times.Once);
        }
        [TestMethod]
        public void WhenInsertingSavingsAssetAccountData_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount actualSavingsAssetAccount = (SavingsAssetAccount)accountRepo.InsertAccount(expectedSavingsAssetAccount);

            mockDataLayer.Verify(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfSavingsAssetAccountInsertValues &&
                (string)y[DataObjects.Accounts.Columns.Name] == expectedSavingsAssetAccount.Name &&
                (AccountType)y[DataObjects.Accounts.Columns.AccountType] == expectedSavingsAssetAccount.AccountType &&
                (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedSavingsAssetAccount.Balance &&
                (AssetAccountType)y[DataObjects.Accounts.Columns.AssetAccountType] == expectedSavingsAssetAccount.AssetAccountType &&
                (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedSavingsAssetAccount.InterestRate)),
            Times.Once);
        }
        [TestMethod]
        public void WhenInsertingSavingsAssetAccountData_CorrectSavingsAssetAccountObjectIsReturned()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount newAccount = CreateExpectedSavingsAssetAccount();
            newAccount.Id = 0;
            newAccount.StatementTimePeriod.Id = 0;
            newAccount.StatementTimePeriod.OwnerAccountId = 0;
            SavingsAssetAccount actualSavingsAssetAccount = (SavingsAssetAccount)accountRepo.InsertAccount(newAccount);

            Assert.AreEqual(expectedSavingsAssetAccount.Id, actualSavingsAssetAccount.Id);
            Assert.AreEqual(expectedSavingsAssetAccount.StatementTimePeriod.OwnerAccountId, actualSavingsAssetAccount.StatementTimePeriod.OwnerAccountId);
        }
        [TestMethod]
        public void WhenInsertingSavingsAssetAccountData_CorrectTimePeriodRepoCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount newAccount = CreateExpectedSavingsAssetAccount();
            newAccount.Id = 0;
            newAccount.StatementTimePeriod.Id = 0;
            newAccount.StatementTimePeriod.OwnerAccountId = 0;
            SavingsAssetAccount actualSavingsAssetAccount = (SavingsAssetAccount)accountRepo.InsertAccount(newAccount);

            mockTimePeriodRepo.Verify(x => x.InsertTimePeriod(
                It.Is<TimePeriod>(y =>
                    y.Name == expectedSavingsAssetAccount.StatementTimePeriod.Name &&
                    y.LastOccurance == expectedSavingsAssetAccount.StatementTimePeriod.LastOccurance &&
                    y.PeriodMethod == expectedSavingsAssetAccount.StatementTimePeriod.PeriodMethod &&
                    y.PeriodType == expectedSavingsAssetAccount.StatementTimePeriod.PeriodType &&
                    y.PeriodValue == expectedSavingsAssetAccount.StatementTimePeriod.PeriodValue &&
                    y.OwnerAccountId == expectedSavingsAssetAccount.Id)),
                Times.Once);
        }
        #endregion

        #region Asset Account Editing Tests
        [TestMethod]
        public void WhenUpdatingCashAssetAccountData_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CashAssetAccount actualCashAssetAccount = (CashAssetAccount)accountRepo.UpdateAccount(expectedCashAssetAccount);

            mockDataLayer.Verify(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCashAssetAccountUpdateValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedCashAssetAccount.Name &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCashAssetAccount.Balance),
                It.Is<int>(y => y == expectedCashAssetAccount.Id)),
                Times.Once);
        }
        [TestMethod]
        public void WhenUpdatingCashAssetAccountData_CorrectCashAssetAccountObjectIsReturnedBasedOnId()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CashAssetAccount actualCashAssetAccount = (CashAssetAccount)accountRepo.UpdateAccount(expectedCashAssetAccount);

            Assert.AreEqual(expectedCashAssetAccount.Id, actualCashAssetAccount.Id);
        }
        [TestMethod]
        public void WhenUpdatingCreditAssetAccountData_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount actualCreditAssetAccount = (CreditAssetAccount)accountRepo.UpdateAccount(expectedCreditAssetAccount);

            mockDataLayer.Verify(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCreditAssetAccountUpdateValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedCreditAssetAccount.Name &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCreditAssetAccount.Balance &&
                    (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedCreditAssetAccount.InterestRate &&
                    (decimal)y[DataObjects.Accounts.Columns.CreditLine] == expectedCreditAssetAccount.CreditLine),
                It.Is<int>(y => y == expectedCreditAssetAccount.Id)),
                Times.Once);
        }
        [TestMethod]
        public void WhenUpdatingCreditAssetAccountData_CorrectCreditAssetAccountObjectIsReturnedBasedOnId()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount actualCreditAssetAccount = (CreditAssetAccount)accountRepo.UpdateAccount(expectedCreditAssetAccount);

            Assert.AreEqual(expectedCreditAssetAccount.Id, actualCreditAssetAccount.Id);
        }
        [TestMethod]
        public void WhenUpdatingSavingsAssetAccountData_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount actualSavingsAssetAccount = (SavingsAssetAccount)accountRepo.UpdateAccount(expectedSavingsAssetAccount);

            mockDataLayer.Verify(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfSavingsAssetAccountUpdateValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedSavingsAssetAccount.Name &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedSavingsAssetAccount.Balance &&
                    (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedSavingsAssetAccount.InterestRate),
                It.Is<int>(y => y == expectedSavingsAssetAccount.Id)),
                Times.Once);
        }
        [TestMethod]
        public void WhenUpdatingSavingsAssetAccountData_CorrectCreditAssetAccountObjectIsReturnedBasedOnId()
        {
            Mock<IDataLayer> mockDataLayer = MockAssetAccountRequests();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = MockTimePeriodRepoRequests();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            SavingsAssetAccount actualSavingsAssetAccount = (SavingsAssetAccount)accountRepo.UpdateAccount(expectedSavingsAssetAccount);

            Assert.AreEqual(expectedSavingsAssetAccount.Id, actualSavingsAssetAccount.Id);
        }
        
        #endregion

        #region Asset Account Removal Tests
        [TestMethod]
        public void WhenDeletingAssetAccount_CorrectAccountDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = new Mock<IDataLayer>();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = new Mock<ITimePeriodRepo>();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            accountRepo.DeleteAccount(expectedDeleteAssetAccount);

            mockDataLayer.Verify(x => x.DeleteRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedDeleteAssetAccount.Id)), Times.Once);
        }
        [TestMethod]
        public void WhenDeletingAssetAccount_CorrectAccountToTimePeriodLinkDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = new Mock<IDataLayer>();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = new Mock<ITimePeriodRepo>();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            accountRepo.DeleteAccount(expectedDeleteCreditAssetAccount);

            mockDataLayer.Verify(x => x.DeleteRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
                It.Is<Dictionary<string, object>>(y => 
                    (int)y[DataObjects.AccountToTimePeriodLink.Columns.AccountId] == expectedDeleteCreditAssetAccount.Id)), 
                Times.Once);
        }
        [TestMethod]
        public void WhenDeletingCreditAssetAccountWithStatementTimePeriod_CorrectTimePeriodRepoCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = new Mock<IDataLayer>();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = new Mock<ITimePeriodRepo>();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            accountRepo.DeleteAccount(expectedDeleteCreditAssetAccount);

            mockTimePeriodRepo.Verify(x => x.DeleteTimePeriod(
                It.Is<int>(y => y == expectedDeleteCreditAssetAccount.StatementTimePeriod.Id)),
                Times.Once);
        }
        [TestMethod]
        public void WhenDeletingSavingsAssetAccountWithStatementTimePeriod_CorrectTimePeriodRepoCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = new Mock<IDataLayer>();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = new Mock<ITimePeriodRepo>();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            accountRepo.DeleteAccount(expectedDeleteSavingsAssetAccount);

            mockTimePeriodRepo.Verify(x => x.DeleteTimePeriod(
                It.Is<int>(y => y == expectedDeleteSavingsAssetAccount.StatementTimePeriod.Id)),
                Times.Once);
        }
        [TestMethod]
        public void WhenDeletingCreditOrSavingsAssetAccountWithNoStatementTimePeriod_NoTimePeriodRepoCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = new Mock<IDataLayer>();
            Mock<ITimePeriodRepo> mockTimePeriodRepo = new Mock<ITimePeriodRepo>();
            IAccountRepo accountRepo = new AssetAccountRepo(mockDataLayer.Object, mockTimePeriodRepo.Object);
            CreditAssetAccount oldCreditAccount = CreateExpectedCreditAssetAccount();
            oldCreditAccount.StatementTimePeriod = null;
            SavingsAssetAccount oldSavingsAccount = CreateExpectedSavingsAssetAccount();
            oldSavingsAccount.StatementTimePeriod = null;

            accountRepo.DeleteAccount(oldCreditAccount);
            accountRepo.DeleteAccount(oldSavingsAccount);

            mockTimePeriodRepo.Verify(x => x.DeleteTimePeriod(
                It.IsAny<int>()),
                Times.Never);
        }
        #endregion

        #region Moq Helper Methods
        private Mock<IDataLayer> MockAssetAccountRequests()
        {
            Mock<IDataLayer> dataLayer = new Mock<IDataLayer>();
            AddCashAssetRequestsToDataLayer(ref dataLayer);
            AddCreditAssetRequestsToDataLayer(ref dataLayer);
            AddSavingsAssetRequestsToDataLayer(ref dataLayer);            
            return dataLayer;
        }
        private void AddCashAssetRequestsToDataLayer(ref Mock<IDataLayer> dataLayer)
        {
            //Get Cash Asset Account
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCashAssetAccount.Id)))
                .Returns(CreateMockCashAssetAccountDataTable());
            //Insert Cash Asset Account
            dataLayer.Setup(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCashAssetAccountInsertValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedCashAssetAccount.Name &&
                    (AccountType)y[DataObjects.Accounts.Columns.AccountType] == expectedCashAssetAccount.AccountType &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCashAssetAccount.Balance &&
                    (AssetAccountType)y[DataObjects.Accounts.Columns.AssetAccountType] == expectedCashAssetAccount.AssetAccountType)))
                .Returns(CreateMockCashAssetAccountDataTable());
            //Update Cash Asset Account
            dataLayer.Setup(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCashAssetAccountUpdateValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedCashAssetAccount.Name &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCashAssetAccount.Balance),
                It.Is<int>(y => y == expectedCashAssetAccount.Id)))
                .Returns(CreateMockCashAssetAccountDataTable());
        }
        private void AddCreditAssetRequestsToDataLayer(ref Mock<IDataLayer> dataLayer)
        {
            //Get Credit Asset Account
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedCreditAssetAccount.Id))).Returns(CreateMockCreditAssetAccountDataTable());
            //Get Linked Time Period
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfAccountToTimePeriodLinkGetValues &&
                    (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedCreditAssetAccount.Id)))
                .Returns(CreateMockExampleTimePeriodDataTable(expectedOwnedCreditTimePeriod));
            //Insert Credit Asset Account
            dataLayer.Setup(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCreditAssetAccountInsertValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedCreditAssetAccount.Name &&
                    (AccountType)y[DataObjects.Accounts.Columns.AccountType] == expectedCreditAssetAccount.AccountType &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCreditAssetAccount.Balance &&
                    (AssetAccountType)y[DataObjects.Accounts.Columns.AssetAccountType] == expectedCreditAssetAccount.AssetAccountType &&
                    (decimal)y[DataObjects.Accounts.Columns.CreditLine] == expectedCreditAssetAccount.CreditLine &&
                    (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedCreditAssetAccount.InterestRate)))
                .Returns(CreateMockCreditAssetAccountDataTable());
            //Update Credit Asset Account
            dataLayer.Setup(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfCreditAssetAccountUpdateValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedCreditAssetAccount.Name &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedCreditAssetAccount.Balance &&
                    (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedCreditAssetAccount.InterestRate &&
                    (decimal)y[DataObjects.Accounts.Columns.CreditLine] == expectedCreditAssetAccount.CreditLine),
                It.Is<int>(y => y == expectedCreditAssetAccount.Id)))
                .Returns(CreateMockCreditAssetAccountDataTable());
        }
        private void AddSavingsAssetRequestsToDataLayer(ref Mock<IDataLayer> dataLayer)
        {
            //Get Savings Asset Account
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<int>(y => y == expectedSavingsAssetAccount.Id))).Returns(CreateMockSavingsAssetAccountDataTable());
            //Get Linked Time Period
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfAccountToTimePeriodLinkGetValues &&
                    (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedSavingsAssetAccount.Id)))
                .Returns(CreateMockExampleTimePeriodDataTable(expectedOwnedSavingsTimePeriod));
            //Insert Savings Asset Account
            dataLayer.Setup(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfSavingsAssetAccountInsertValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedSavingsAssetAccount.Name &&
                    (AccountType)y[DataObjects.Accounts.Columns.AccountType] == expectedSavingsAssetAccount.AccountType &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedSavingsAssetAccount.Balance &&
                    (AssetAccountType)y[DataObjects.Accounts.Columns.AssetAccountType] == expectedSavingsAssetAccount.AssetAccountType &&
                    (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedSavingsAssetAccount.InterestRate)))
                .Returns(CreateMockSavingsAssetAccountDataTable());
            //Update Savings Asset Account
            dataLayer.Setup(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.Account),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfSavingsAssetAccountUpdateValues &&
                    (string)y[DataObjects.Accounts.Columns.Name] == expectedSavingsAssetAccount.Name &&
                    (decimal)y[DataObjects.Accounts.Columns.Balance] == expectedSavingsAssetAccount.Balance &&
                    (decimal)y[DataObjects.Accounts.Columns.InterestRate] == expectedSavingsAssetAccount.InterestRate),
                It.Is<int>(y => y == expectedSavingsAssetAccount.Id)))
                .Returns(CreateMockSavingsAssetAccountDataTable());
        }
        private Mock<ITimePeriodRepo> MockTimePeriodRepoRequests()
        {
            Mock<ITimePeriodRepo> timePeriodRepo = new Mock<ITimePeriodRepo>();
            //Credit Asset Account Associated Time Period
            timePeriodRepo.Setup(x => x.GetTimePeriod(
                It.Is<int>(y => y == expectedOwnedCreditTimePeriod.Id)))
                .Returns(expectedOwnedCreditTimePeriod);
            //Savings Asset Account Associated Time Period
            timePeriodRepo.Setup(x => x.GetTimePeriod(
                It.Is<int>(y => y == expectedOwnedSavingsTimePeriod.Id)))
                .Returns(expectedOwnedSavingsTimePeriod);
            return timePeriodRepo;
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
            newAccount.StatementTimePeriod = CreateExpectedOwnedCreditTimePeriod();
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

            newAccount.StatementTimePeriod = CreateExpectedOwnedSavingsTimePeriod();
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
            newTimePeriod.Id = 12;
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
