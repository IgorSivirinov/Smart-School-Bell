using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using SmartSchoolBellCore.Model;

namespace SmartSchoolBellCore.Extension
{
    public static class StringDayOfWeekExtension
    {
        public static async Task<TimetableDayOfWeek> GetTimetableDayOfWeekAsync(this string dayOfWeekString,
            DatabaseContext context,
            int id)
        {
            var timetable = await context.Timetables.Include(i => i.TimetableDayOfWeeks).Include(i => i.TimetableDayOfWeeks)
                .SingleAsync(i => i.Id == id);
            return timetable.TimetableDayOfWeeks[dayOfWeekString switch
            {
                "Пн." => 1,
                "Вт." => 2,
                "Ср." => 3,
                "Чт." => 4,
                "Пт." => 5,
                "Сб." => 6,
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeekString), dayOfWeekString, null)
            }];
        }

    }
}