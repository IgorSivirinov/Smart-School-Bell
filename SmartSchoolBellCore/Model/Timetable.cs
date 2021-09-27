using System;
using System.Collections.Generic;

namespace SmartSchoolBellCore.Model
{
    public class Timetable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UriFile { get; set; }
        public bool Working { get; set; }

        public List<TimetableDayOfWeek> TimetableDayOfWeeks { get; set; }

        public Timetable(string name)
        {
            Name = name;
        
            Working = true;

            TimetableDayOfWeeks = new()
            {
                new(DayOfWeek.Sunday),
                new (DayOfWeek.Monday),
                new (DayOfWeek.Tuesday),
                new (DayOfWeek.Wednesday),
                new (DayOfWeek.Thursday),
                new (DayOfWeek.Friday),
                new(DayOfWeek.Saturday)
            };
        }

        public Timetable() { }
    }
}