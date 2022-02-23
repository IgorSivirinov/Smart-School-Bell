using System;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using Microsoft.EntityFrameworkCore;
using SmartSchoolBellCore.Model;

namespace SmartSchoolBellCore.Services
{
    public static class StartBellService
    {
        public static async Task StartOperationIfThereTimetable (DatabaseContext context, TimeSpan timeSpanCurrent, DayOfWeek dayOfWeekNow, Action<TimeSpan, TimeSpan, Uri> operation)
        {
            var passCounter = 0;
            var dateNowNumber = (int) dayOfWeekNow;
            while (passCounter <= 7)
            {
                var totalMinutes = timeSpanCurrent.TotalMinutes;
                var number = dateNowNumber;
                var timesFromCurrentDayOfWeek =
                    await context.TimetableDayOfWeeks
                        .Include(i => i.Timetable)
                        .Include(i => i.TimeBells)
                        .Where(timetable => timetable.Timetable.Working)
                        .Where(timetable => timetable.DayOfWeekNumber == number)
                        .Where(
                            timetable => 
                                timetable.TimeBells.Any(
                                    timeBell => passCounter == 0 ? (timeBell.Hour * 60 + timeBell.Min > totalMinutes) : true))
                        .ToListAsync();

                if (timesFromCurrentDayOfWeek.Count != 0)
                {
                    var timetableDayOfWeek = timesFromCurrentDayOfWeek.OrderBy(timetableDayOfWeek =>
                        {
                            var timeBell = timetableDayOfWeek.TimeBells
                                .Where(timeBell => passCounter == 0 ? (timeBell.Hour * 60 + timeBell.Min > totalMinutes) : true)
                                .OrderBy(timeBell => timeBell.Hour * 60 + timeBell.Min).First();
                            return timeBell.Hour * 60 + timeBell.Min;
                        })
                        .First();

                    var minTime = timetableDayOfWeek.TimeBells.Where(timeBell =>
                            passCounter == 0 ? (timeBell.Hour * 60 + timeBell.Min > totalMinutes) : true)
                        .OrderBy(timeBell => timeBell.Hour * 60 + timeBell.Min).First();

                    var nextTime = new TimeSpan(minTime.Hour, minTime.Min, 0);

                    minTime.Hour += 24 * passCounter;

                    var currentUri = new Uri(timetableDayOfWeek.Timetable.UriFile);

                    operation(nextTime, new TimeSpan(minTime.Hour, minTime.Min, 0).Subtract(timeSpanCurrent), currentUri);
                    break;
                }
                passCounter++;
                dateNowNumber++;
                if (dateNowNumber > 6) dateNowNumber = 0;
            }
        }

        public static async Task StartTimerBell(Dispatcher _dispatcher = null)
        {
            var dateNow = DateTime.Now;
            var timeSpanNow = new TimeSpan(dateNow.Hour, dateNow.Minute, dateNow.Second);
            await using var context = new DatabaseContext();
            await StartOperationIfThereTimetable(context, timeSpanNow, dateNow.DayOfWeek, (nextTime, time, uri) =>
            {
                    App.StaticDispatcher.BeginInvoke(() =>
                    {
                        //MessageBox.Show($"{time}", "ete", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (App.BellObservableDisposable != null) App.BellObservableDisposable.Dispose();
                        time.Add(new(0, 0, 2));
                        App.BellObservableDisposable = Observable.Timer(time)
                        .Subscribe(t =>
                        {
                            var dateNow = DateTime.Now;
                            var timeSpanNow = new TimeSpan(dateNow.Hour, dateNow.Minute, dateNow.Second);

                            if (timeSpanNow.TotalSeconds - timeSpanNow.TotalSeconds - 2 <= 0)
                                App.StaticDispatcher.BeginInvoke(() => App.Player.Open(uri));
                            
                            Task.Run(async () => await StartTimerBell());
                        });
                    });
            });
        }
    }
}