using System.Collections.Generic;
using System.Data;

namespace PloutosMain.DataLayer
{
    public interface IDataLayer
    {
        DataTable GetRecord(DataEnums.TargetObject targetObject, int targetId);
        DataTable InsertRecord(DataEnums.TargetObject targetObject, Dictionary<string, object> valueList);
        DataTable UpdateRecords(DataEnums.TargetObject targetObject, Dictionary<string, object> valueList, Dictionary<string, object> criteriaList);
        void DeleteRecord(DataEnums.TargetObject targetObject, int targetId);
    }
}
