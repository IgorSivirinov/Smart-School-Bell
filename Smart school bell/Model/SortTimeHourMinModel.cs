using System.Collections.Generic;

namespace Smart_school_bell.Model
{
    public class SortTimeHourMinModel : IComparer<TimetableDayOfWeek.TimeHourMin>
    {

        public int Compare(TimetableDayOfWeek.TimeHourMin x, TimetableDayOfWeek.TimeHourMin y)
        {
            if (x.Time > y.Time) 
                return 1;

            if (x.Time < y.Time)
                return -1;

            return 0;
        }
    }
}