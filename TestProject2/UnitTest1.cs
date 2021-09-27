using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSchoolBellCore.Model;
using SmartSchoolBellCore.Services;

namespace TestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            await SetDataAndTestAsync(DayOfWeek.Monday, new(15, 30),
                DayOfWeek.Monday,new(13, 30, 0),
                mins => 
                    mins == 120);
            await SetDataAndTestAsync(DayOfWeek.Friday,
                new(15, 30),
                DayOfWeek.Monday,new(13, 30, 0),
                mins => mins == 120 + 4 * 24 * 60);
            await SetDataAndTestAsync(DayOfWeek.Monday,
                new(15, 30),
                DayOfWeek.Tuesday, new(13, 30, 0),
                mins => 
                    mins == 120 + 6 * 24 * 60);
        }

        public async Task SetDataAndTestAsync(DayOfWeek dayOfWeekNew, TimeHourMin timeHourMinNew,
            DayOfWeek dayOfWeekCurrent,TimeSpan testTimeCurrent, Func<int, bool> func)
        {
            var context = new DatabaseContext("Data Source=start_timer_ball.db");
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var timetable = new Timetable("TestName")
            {
                UriFile = "C:\\Users\\user\\Music\\Sound_20947.mp3"
            };
            await context.Timetables.AddAsync(timetable);
            await context.SaveChangesAsync();

            for (var i = 0; i < 6; i++)
                (await context.Timetables.FindAsync(timetable.Id)).TimetableDayOfWeeks[i].TimetableId
                    = (await context.Timetables.FindAsync(timetable.Id)).Id;

            timeHourMinNew.TimetableDayOfWeekId = timetable.TimetableDayOfWeeks[(int) dayOfWeekNew].Id;
            await context.TimeHourMins.AddAsync(timeHourMinNew);
            await context.SaveChangesAsync();

            await StartBellService.StartOperationIfThereTimetable(context, new TimeSpan(13, 30, 0), dayOfWeekCurrent,
                (t, uri) =>
                    Assert.IsTrue(func((int)t.TotalMinutes)));
        }
    }
}
