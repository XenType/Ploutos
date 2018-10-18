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
    public class TImePeriodRepoTests
    {
        #region Test Mock Objects
        private TimePeriod expectedNotFoundTimePeriod;
        private TimePeriod expectedExampleTimePeriod;
        private TimePeriod expectedUpdatedTimePeriod;
        private TimePeriodNotFoundException expectedTimePeriodNotFoundException;
        #endregion

        #region Test Constants
        private const int expectedNumberOfTimePeriodInsertValues = 6;
        private const int expectedNumberOfTimePeriodUpdateValues = 6;
        private const int expectedDeleteTimePeriodId = 11;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            expectedNotFoundTimePeriod = CreateExpectedNotFoundTimePeriod();
            expectedExampleTimePeriod = CreateExpectedExampleTimePeriod();
            expectedUpdatedTimePeriod = CreateUpdatedExampleTimePeriod();
            expectedTimePeriodNotFoundException = new TimePeriodNotFoundException(expectedNotFoundTimePeriod.Id);
        }
        #endregion

        #region Exception Tests  
        [TestMethod]
        public void WhenTimePeriodDoesNotExist_CorrectExceptionIsThrown()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            Exception actualException = new Exception();
            try
            {
                TimePeriod actualTimePeriod = timePeriodRepo.GetTimePeriod(expectedNotFoundTimePeriod.Id);
            }
            catch (Exception e)
            {
                actualException = e;
            }
            Assert.AreEqual(expectedTimePeriodNotFoundException.Message, actualException.Message);
        }
        #endregion

        #region Time Period Retrieval Tests
        // DataLayer command usage
        [TestMethod]
        public void WhenGettingTimePeriod_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod actualTimePeriod = timePeriodRepo.GetTimePeriod(expectedExampleTimePeriod.Id);
            /// Expectations
            /// A call will be made to retrieve the asset account data
            mockDataLayer.Verify(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<int>(y => y == expectedExampleTimePeriod.Id)), Times.Once);
            /// A call will be made to retrieve a list of account ids linked to this time period
            mockDataLayer.Verify(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
                It.Is<Dictionary<string, object>>(y => (int)y[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] == expectedExampleTimePeriod.Id)), 
                Times.Once);
        }
        // DataTable Mapping
        [TestMethod]
        public void WhenGettingTimePeriodBasicData_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod actualTimePeriod = timePeriodRepo.GetTimePeriod(expectedExampleTimePeriod.Id);

            Assert.AreEqual(expectedExampleTimePeriod.Id, actualTimePeriod.Id);
            Assert.AreEqual(expectedExampleTimePeriod.Name, actualTimePeriod.Name);
            Assert.AreEqual(expectedExampleTimePeriod.LastOccurance, actualTimePeriod.LastOccurance);
            Assert.AreEqual(expectedExampleTimePeriod.PeriodMethod, actualTimePeriod.PeriodMethod);
            Assert.AreEqual(expectedExampleTimePeriod.PeriodType, actualTimePeriod.PeriodType);
            Assert.AreEqual(expectedExampleTimePeriod.PeriodValue, actualTimePeriod.PeriodValue);
            Assert.AreEqual(expectedExampleTimePeriod.OwnerAccountId, actualTimePeriod.OwnerAccountId);
        }
        [TestMethod]
        public void WhenGettingTimePeriodBasicData_CorrectLinkedAccountsAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod actualTimePeriod = timePeriodRepo.GetTimePeriod(expectedExampleTimePeriod.Id);

            Assert.AreEqual(expectedExampleTimePeriod.LinkedAccountList.Count, actualTimePeriod.LinkedAccountList.Count);
            Assert.AreEqual(expectedExampleTimePeriod.LinkedAccountList[0], actualTimePeriod.LinkedAccountList[0]);
            Assert.AreEqual(expectedExampleTimePeriod.LinkedAccountList[1], actualTimePeriod.LinkedAccountList[1]);
        }

        #endregion

        #region Time Period Creation Tests
        [TestMethod]
        public void WhenInsertingTimePeriodData_CorrectTimePeriodDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod newTimePeriod = CreateExpectedExampleTimePeriod();
            TimePeriod actualTimePeriod = timePeriodRepo.InsertTimePeriod(newTimePeriod);

            mockDataLayer.Verify(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfTimePeriodInsertValues &&
                     (string)y[DataObjects.TimePeriods.Columns.Name] == expectedExampleTimePeriod.Name &&
                     (DateTime)y[DataObjects.TimePeriods.Columns.LastOccurance] == expectedExampleTimePeriod.LastOccurance &&
                     (PeriodMethod)y[DataObjects.TimePeriods.Columns.PeriodMethod] == expectedExampleTimePeriod.PeriodMethod &&
                     (PeriodType)y[DataObjects.TimePeriods.Columns.PeriodType] == expectedExampleTimePeriod.PeriodType &&
                     (int)y[DataObjects.TimePeriods.Columns.PeriodValue] == expectedExampleTimePeriod.PeriodValue &&
                     (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedExampleTimePeriod.OwnerAccountId)),
                Times.Once);
        }

        [TestMethod]
        public void WhenInsertingTimePeriodData_CorrectTimePeriodObjectIsReturned()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod newTimePeriod = CreateExpectedExampleTimePeriod();
            TimePeriod actualTimePeriod = timePeriodRepo.InsertTimePeriod(newTimePeriod);

            Assert.AreEqual(expectedExampleTimePeriod.Id, actualTimePeriod.Id);
            Assert.AreEqual(0, actualTimePeriod.LinkedAccountList.Count);
        }
        #endregion

        #region Time Period Editing Tests
        [TestMethod]
        public void WhenUpdatingTimePeriodData_CorrectTimePeriodDbCallIsMade()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod updatedTimePeriod = CreateUpdatedExampleTimePeriod();
            TimePeriod actualTimePeriod = timePeriodRepo.UpdateTimePeriod(updatedTimePeriod);
            /// Expectations
            /// A call will be made to save the asset account data
            mockDataLayer.Verify(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfTimePeriodUpdateValues &&
                     (string)y[DataObjects.TimePeriods.Columns.Name] == expectedUpdatedTimePeriod.Name &&
                     (DateTime)y[DataObjects.TimePeriods.Columns.LastOccurance] == expectedUpdatedTimePeriod.LastOccurance &&
                     (PeriodMethod)y[DataObjects.TimePeriods.Columns.PeriodMethod] == expectedUpdatedTimePeriod.PeriodMethod &&
                     (PeriodType)y[DataObjects.TimePeriods.Columns.PeriodType] == expectedUpdatedTimePeriod.PeriodType &&
                     (int)y[DataObjects.TimePeriods.Columns.PeriodValue] == expectedUpdatedTimePeriod.PeriodValue &&
                     (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedUpdatedTimePeriod.OwnerAccountId),
                It.Is<int>(y => y == expectedUpdatedTimePeriod.Id)),
                Times.Once);
            /// A call will be made to retrieve a list of account ids linked to this time period
            mockDataLayer.Verify(x => x.GetRecords(
               It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
               It.Is<Dictionary<string, object>>(y => (int)y[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] == expectedUpdatedTimePeriod.Id)),
               Times.Once);
        }
        [TestMethod]
        public void WhenUpdatingTimePeriodData_CorrectPropertiesAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod updatedTimePeriod = CreateUpdatedExampleTimePeriod();
            TimePeriod actualTimePeriod = timePeriodRepo.UpdateTimePeriod(updatedTimePeriod);

            Assert.AreEqual(expectedUpdatedTimePeriod.Id, actualTimePeriod.Id);
            Assert.AreEqual(expectedUpdatedTimePeriod.Name, actualTimePeriod.Name);
            Assert.AreEqual(expectedUpdatedTimePeriod.LastOccurance, actualTimePeriod.LastOccurance);
            Assert.AreEqual(expectedUpdatedTimePeriod.PeriodMethod, actualTimePeriod.PeriodMethod);
            Assert.AreEqual(expectedUpdatedTimePeriod.PeriodType, actualTimePeriod.PeriodType);
            Assert.AreEqual(expectedUpdatedTimePeriod.PeriodValue, actualTimePeriod.PeriodValue);
            Assert.AreEqual(expectedUpdatedTimePeriod.OwnerAccountId, actualTimePeriod.OwnerAccountId);
        }
        [TestMethod]
        public void WhenUpdatingTimePeriodData_CorrectLinkedAccountsAreMapped()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            TimePeriod updatedTimePeriod = CreateUpdatedExampleTimePeriod();
            TimePeriod actualTimePeriod = timePeriodRepo.UpdateTimePeriod(updatedTimePeriod);

            Assert.AreEqual(expectedUpdatedTimePeriod.LinkedAccountList.Count, actualTimePeriod.LinkedAccountList.Count);
            for (int i = 0; i < expectedUpdatedTimePeriod.LinkedAccountList.Count; i++)
                Assert.AreEqual(expectedUpdatedTimePeriod.LinkedAccountList[i], actualTimePeriod.LinkedAccountList[i]);
            
        }
        #endregion

        #region Time Period Removal Tests
        [TestMethod]
        public void WhenDeletingTimePeriod_CorrectDbCallsAreMade()
        {
            Mock<IDataLayer> mockDataLayer = MockTimePeriodRequests();
            ITimePeriodRepo timePeriodRepo = new TimePeriodRepo(mockDataLayer.Object);
            timePeriodRepo.DeleteTimePeriod(expectedDeleteTimePeriodId);
            /// Expectations
            /// A single call will be made to delete all associated account links
            mockDataLayer.Verify(x => x.DeleteRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
                It.Is<Dictionary<string,object>>(y => (int)y[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] == expectedDeleteTimePeriodId)), 
                Times.Once);
            /// A call will be made to delete the time period data
            mockDataLayer.Verify(x => x.DeleteRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<int>(y => y == expectedDeleteTimePeriodId)),
                Times.Once);
        }
        #endregion

        #region Moq Helper Methods
        private Mock<IDataLayer> MockTimePeriodRequests()
        {
            Mock<IDataLayer> dataLayer = new Mock<IDataLayer>();
            //get
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<int>(y => y == expectedExampleTimePeriod.Id))
                ).Returns(CreateMockExampleTimePeriodDataTable(expectedExampleTimePeriod));
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
                It.Is<Dictionary<string, object>>(y => (int)y[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] == expectedExampleTimePeriod.Id))
                ).Returns(CreateMockExampleAccountToTimePeriodDataTable(expectedExampleTimePeriod.Id, expectedExampleTimePeriod.LinkedAccountList));
            //insert
            dataLayer.Setup(x => x.InsertRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfTimePeriodInsertValues &&
                     (string)y[DataObjects.TimePeriods.Columns.Name] == expectedExampleTimePeriod.Name &&
                     (DateTime)y[DataObjects.TimePeriods.Columns.LastOccurance] == expectedExampleTimePeriod.LastOccurance &&
                     (PeriodMethod)y[DataObjects.TimePeriods.Columns.PeriodMethod] == expectedExampleTimePeriod.PeriodMethod &&
                     (PeriodType)y[DataObjects.TimePeriods.Columns.PeriodType] == expectedExampleTimePeriod.PeriodType &&
                     (int)y[DataObjects.TimePeriods.Columns.PeriodValue] == expectedExampleTimePeriod.PeriodValue &&
                     (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedExampleTimePeriod.OwnerAccountId))
                ).Returns(CreateMockExampleTimePeriodDataTable(expectedExampleTimePeriod));
            //update
            dataLayer.Setup(x => x.UpdateRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<Dictionary<string, object>>(y => y.Count == expectedNumberOfTimePeriodUpdateValues &&
                     (string)y[DataObjects.TimePeriods.Columns.Name] == expectedUpdatedTimePeriod.Name &&
                     (DateTime)y[DataObjects.TimePeriods.Columns.LastOccurance] == expectedUpdatedTimePeriod.LastOccurance &&
                     (PeriodMethod)y[DataObjects.TimePeriods.Columns.PeriodMethod] == expectedUpdatedTimePeriod.PeriodMethod &&
                     (PeriodType)y[DataObjects.TimePeriods.Columns.PeriodType] == expectedUpdatedTimePeriod.PeriodType &&
                     (int)y[DataObjects.TimePeriods.Columns.PeriodValue] == expectedUpdatedTimePeriod.PeriodValue &&
                     (int)y[DataObjects.TimePeriods.Columns.OwnerAccountId] == expectedUpdatedTimePeriod.OwnerAccountId),
                It.Is<int>(y => y == expectedUpdatedTimePeriod.Id))
                ).Returns(CreateMockExampleTimePeriodDataTable(expectedUpdatedTimePeriod));
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
                It.Is<Dictionary<string, object>>(y => (int)y[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] == expectedUpdatedTimePeriod.Id))
                ).Returns(CreateMockExampleAccountToTimePeriodDataTable(expectedUpdatedTimePeriod.Id, expectedUpdatedTimePeriod.LinkedAccountList));
            return dataLayer;
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
        private DataTable CreateMockExampleAccountToTimePeriodDataTable(int timePeriodId, List<int> accountIdList)
        {
            DataTable accountToTimePeriodLinkTable = CreateMockAccountToTimePeriodDataTable();
            foreach (int accountId in accountIdList)
            {
                DataRow newRow = accountToTimePeriodLinkTable.NewRow();
                newRow[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] = timePeriodId;
                newRow[DataObjects.AccountToTimePeriodLink.Columns.AccountId] = accountId;
                accountToTimePeriodLinkTable.Rows.Add(newRow);
            }
            return accountToTimePeriodLinkTable;
        }
        private DataTable CreateMockAccountToTimePeriodDataTable()
        {
            DataTable accountToTimePeriodLinkTable = new DataTable();
            DataColumn newColumn = new DataColumn(DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId, typeof(int));
            accountToTimePeriodLinkTable.Columns.Add(newColumn);
            newColumn = new DataColumn(DataObjects.AccountToTimePeriodLink.Columns.AccountId, typeof(int));
            accountToTimePeriodLinkTable.Columns.Add(newColumn);
            return accountToTimePeriodLinkTable;
        }
        #endregion

        #region Test Helper Methods
        private TimePeriod CreateExpectedNotFoundTimePeriod()
        {
            TimePeriod newTimePeriod = new TimePeriod();
            newTimePeriod.Id = 0;
            return newTimePeriod;
        }
        private TimePeriod CreateExpectedExampleTimePeriod()
        {
            TimePeriod newTimePeriod = new TimePeriod();
            newTimePeriod.Id = 11;
            newTimePeriod.Name = "Biweekly Payday";
            newTimePeriod.LastOccurance = DateTime.Parse("1/1/2018");
            newTimePeriod.PeriodMethod = PeriodMethod.EveryXUnits;
            newTimePeriod.PeriodType = PeriodType.Week;
            newTimePeriod.PeriodValue = 2;
            newTimePeriod.OwnerAccountId = 100;
            newTimePeriod.LinkedAccountList.Add(2);
            newTimePeriod.LinkedAccountList.Add(3);
            return newTimePeriod;
        }
        private TimePeriod CreateUpdatedExampleTimePeriod()
        {
            TimePeriod newTimePeriod = new TimePeriod();
            newTimePeriod.Id = 19;
            newTimePeriod.Name = "Car Payment";
            newTimePeriod.LastOccurance = DateTime.Parse("1/17/2018");
            newTimePeriod.PeriodMethod = PeriodMethod.SameXofUnit;
            newTimePeriod.PeriodType = PeriodType.Month;
            newTimePeriod.PeriodValue = 1;
            newTimePeriod.OwnerAccountId = 103;
            newTimePeriod.LinkedAccountList.Add(203);
            newTimePeriod.LinkedAccountList.Add(303);
            newTimePeriod.LinkedAccountList.Add(403);
            return newTimePeriod;
        }
        #endregion
    }
}
