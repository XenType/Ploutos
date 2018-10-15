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

        private TimePeriodNotFoundException expectedTimePeriodNotFoundException;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            expectedNotFoundTimePeriod = CreateExpectedNotFoundTimePeriod();
            expectedExampleTimePeriod = CreateExpectedExampleTimePeriod();

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

            #endregion

        #region Time Period Editing Tests

            #endregion

        #region Time Period Removal Tests

            #endregion

        #region Moq Helper Methods
        private Mock<IDataLayer> MockTimePeriodRequests()
        {
            Mock<IDataLayer> dataLayer = new Mock<IDataLayer>();
            dataLayer.Setup(x => x.GetRecord(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.TimePeriod),
                It.Is<int>(y => y == expectedExampleTimePeriod.Id))
                ).Returns(CreateMockExampleTimePeriodDataTable());
            dataLayer.Setup(x => x.GetRecords(
                It.Is<DataObjects.DbTarget>(y => y == DataObjects.DbTarget.AccountToTimePeriodLink),
                It.Is<Dictionary<string, object>>(y => (int)y[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] == expectedExampleTimePeriod.Id))
                ).Returns(CreateMockExampleAccountToTimePeriodDataTable());
            return dataLayer;
        }
        private DataTable CreateMockExampleTimePeriodDataTable()
        {
            DataTable exampleTimePeriodTable = CreateMockTimePeriodDataTable();
            DataRow newRow = exampleTimePeriodTable.NewRow();
            newRow[DataObjects.TimePeriods.Columns.Id] = expectedExampleTimePeriod.Id;
            newRow[DataObjects.TimePeriods.Columns.Name] = expectedExampleTimePeriod.Name;
            newRow[DataObjects.TimePeriods.Columns.LastOccurance] = expectedExampleTimePeriod.LastOccurance;
            newRow[DataObjects.TimePeriods.Columns.PeriodMethod] = expectedExampleTimePeriod.PeriodMethod;
            newRow[DataObjects.TimePeriods.Columns.PeriodType] = expectedExampleTimePeriod.PeriodType;
            newRow[DataObjects.TimePeriods.Columns.PeriodValue] = expectedExampleTimePeriod.PeriodValue;
            newRow[DataObjects.TimePeriods.Columns.OwnerAccountId] = expectedExampleTimePeriod.OwnerAccountId;
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
        private DataTable CreateMockExampleAccountToTimePeriodDataTable()
        {
            DataTable accountToTimePeriodLinkTable = CreateMockAccountToTimePeriodDataTable();
            DataRow newRow = accountToTimePeriodLinkTable.NewRow();
            newRow[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] = expectedExampleTimePeriod.Id;
            newRow[DataObjects.AccountToTimePeriodLink.Columns.AccountId] = expectedExampleTimePeriod.LinkedAccountList[0];
            accountToTimePeriodLinkTable.Rows.Add(newRow);
            newRow = accountToTimePeriodLinkTable.NewRow();
            newRow[DataObjects.AccountToTimePeriodLink.Columns.TimePeriodId] = expectedExampleTimePeriod.Id;
            newRow[DataObjects.AccountToTimePeriodLink.Columns.AccountId] = expectedExampleTimePeriod.LinkedAccountList[1];
            accountToTimePeriodLinkTable.Rows.Add(newRow);
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
            newTimePeriod.OwnerAccountId = 1;
            newTimePeriod.LinkedAccountList.Add(2);
            newTimePeriod.LinkedAccountList.Add(3);
            return newTimePeriod;
        }
        #endregion
    }
}
