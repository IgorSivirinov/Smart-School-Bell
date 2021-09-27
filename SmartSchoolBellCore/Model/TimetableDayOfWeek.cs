using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace SmartSchoolBellCore.Model
{
    public class TimetableDayOfWeek
    {
        public int Id { get; set; }

        public int TimetableId { get; set; }
        public virtual Timetable Timetable { get; set; }

        public List<TimeHourMin> TimeBells { get; set; }

        public int DayOfWeekNumber { get; set; }
        
        [NotMapped]
        public DayOfWeek DayOfWeek
        {
            get => (DayOfWeek) DayOfWeekNumber;
            set => DayOfWeekNumber = (int) value;
        }

        public TimetableDayOfWeek(DayOfWeek dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
        }

        public TimetableDayOfWeek() { }

    }
}