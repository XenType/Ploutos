using PloutosMain.Models;

namespace PloutosMain.Repositories
{
    public interface ITimePeriodRepo
    {
        TimePeriod GetTimePeriod(int timePeriodId);
        TimePeriod InsertTimePeriod(TimePeriod newTimePeriod);
        TimePeriod UpdateTimePeriod(TimePeriod modifiedTimePeriod);
        void DeleteTimePeriod(int timePeriodId);
    }
}
