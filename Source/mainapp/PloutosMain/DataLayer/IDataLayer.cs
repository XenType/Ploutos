using System.Collections.Generic;
using System.Data;

namespace PloutosMain.DataLayer
{
    public interface IDataLayer
    {
        DataTable GetRecord(DataObjects.DbTarget targetObject, int targetId);
        DataTable InsertRecord(DataObjects.DbTarget targetObject, Dictionary<string, object> valueList);
        DataTable UpdateRecords(DataObjects.DbTarget targetObject, Dictionary<string, object> valueList, Dictionary<string, object> criteriaList);
        void DeleteRecord(DataObjects.DbTarget targetObject, int targetId);
    }
}
