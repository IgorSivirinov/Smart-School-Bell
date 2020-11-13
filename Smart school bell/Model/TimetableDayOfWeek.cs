using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Smart_school_bell.Model
{
    public class TimetableDayOfWeek
    {
        public int Id { get; set; }

        public int? TimetableId { get; set; }
        public virtual Timetable Timetable { get; set; }

        [NotMapped]
        public List<TimeHourMin> TimeBells
        {
            get
            {
                
                return JsonSerializer.Deserialize<List<TimeHourMin>>(TimeBellJSON);
            }
            set
            {
                TimeBellJSON = JsonSerializer.Serialize(value);
            }
        }

        public string TimeBellJSON { get; set; }

        [NotMapped]
        public DayOfWeek DayOfWeek
        {
            get
            {
                return JsonSerializer.Deserialize<DayOfWeek>(DayOfWeekJSON);
            }
            set
            {
                DayOfWeekJSON = JsonSerializer.Serialize(value);
            }
        }

        public string DayOfWeekJSON { get; set; }

        public TimetableDayOfWeek(DayOfWeek dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
            TimeBellJSON = "[]";
        }

        public TimetableDayOfWeek()
        {
            TimeBellJSON = "[]";
        }

        public class TimeHourMin
        {
            public int Hour { get; set; }
            public int Min { get; set; }
            [JsonIgnore]
            public string Uri { get; set; }
            [JsonIgnore]
            public int СallDuration { get; set; }

            [JsonIgnore]
            public TimeSpan Time
            {
                get
                {
                    return new TimeSpan(Hour,Min,0);
                }
            }

            public TimeHourMin(int hour, int min)
            {
                Hour = hour;
                Min = min;
            }
            public TimeHourMin(){}
        }
    }
}