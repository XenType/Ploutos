using System.Collections.Generic;
using System.Data;

namespace PloutosMain.DataLayer
{
    public interface IDataLayer
    {
        DataTable GetRecord(DataObjects.DbTarget targetObject, int targetId);
        DataTable GetRecords(DataObjects.DbTarget targetObject, Dictionary<string, object> criteriaList);
        DataTable InsertRecord(DataObjects.DbTarget targetObject, Dictionary<string, object> valueList);
        DataTable UpdateRecord(DataObjects.DbTarget targetObject, Dictionary<string, object> valueList, int targetId);
        DataTable UpdateRecords(DataObjects.DbTarget targetObject, Dictionary<string, object> valueList, Dictionary<string, object> criteriaList);
        void DeleteRecord(DataObjects.DbTarget targetObject, int targetId);
        void DeleteRecords(DataObjects.DbTarget targetObject, Dictionary<string, object> criteriaList);
    }
}
