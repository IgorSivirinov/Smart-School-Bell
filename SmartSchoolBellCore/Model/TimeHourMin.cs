using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSchoolBellCore.Model
{
    public class TimeHourMin
    {
        public int Id { get; set; }

        public int TimetableDayOfWeekId { get; set; }
        public TimetableDayOfWeek TimetableDayOfWeek { get; set; }

        public int Hour { get; set; }

        public int Min { get; set; }

        public TimeHourMin(int hour, int min)
        {
            Hour = hour;
            Min = min;
        }

        public TimeHourMin() { }
    }
}