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
        public static IDisposable BellObservableDisposable { get; set; }

        public static async Task StartOperationIfThereTimetable (DatabaseContext context, TimeSpan timeSpanCurrent, DayOfWeek dayOfWeekNow, Action<TimeSpan, Uri> operation)
        {
            var passCounter = 0;
            var dateNowNumber = (int) dayOfWeekNow;
            while (passCounter < 7)
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
                                    timeBell =>
                                        timeBell.Hour * 60 + timeBell.Min > totalMinutes))
                        .ToListAsync();

                if (timesFromCurrentDayOfWeek.Count != 0)
                {
                    await Task.Delay(2000);
                    var timetableDayOfWeek = timesFromCurrentDayOfWeek.OrderBy(timetableDayOfWeek =>
                        {
                            var timeBell = timetableDayOfWeek.TimeBells
                                .Where(timeBell => timeBell.Hour * 60 + timeBell.Min > totalMinutes)
                                .OrderBy(timeBell => timeBell.Hour * 60 + timeBell.Min).First();
                            return timeBell.Hour * 60 + timeBell.Min;
                        })
                        .First();

                    var minTime = timetableDayOfWeek.TimeBells.Where(timeBell =>
                            timeBell.Hour * 60 + timeBell.Min > totalMinutes)
                        .OrderBy(timeBell => timeBell.Hour * 60 + timeBell.Min).First();

                    minTime.Hour += 24 * passCounter;

                    var currentUri = new Uri(timetableDayOfWeek.Timetable.UriFile);

                    operation(new TimeSpan(minTime.Hour, minTime.Min, 0).Subtract(timeSpanCurrent), currentUri);
                }
                passCounter++;
                dateNowNumber++;
                if (dateNowNumber > 6) dateNowNumber = 0;
            }
        }

        public static async Task  StartTimerBell()
        {
            if (BellObservableDisposable != null) BellObservableDisposable.Dispose();

            var dateNow = DateTime.Now;
            var timeSpanNow = new TimeSpan(dateNow.Hour, dateNow.Minute, dateNow.Second);
            await using var context = new DatabaseContext();
            await StartOperationIfThereTimetable(context, timeSpanNow, dateNow.DayOfWeek, (time, uri) =>
            {
                var player = new MediaPlayer();
                player.MediaFailed += (s, e) =>
                {
                    MessageBox.Show("Проверьте расположение файла",
                        "Ошибка воспроизведения файла (TtSM1)",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                };
                player.Open(uri);
                BellObservableDisposable = Observable.Timer(time)
                    .ObserveOnDispatcher()
                    .SubscribeOn(Scheduler.CurrentThread)
                    .Subscribe(t =>
                    {
                        try
                        {
                            if (!player.HasAudio)
                            {
                                MessageBox.Show("Проверьте расположение файла",
                                    "Ошибка воспроизведения файла (TtSM2)",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            player.Play();

                            StartTimerBell();
                        }
                        catch
                        {
                            MessageBox.Show("Проверьте расположение файла", "Ошибка воспроизведения файла (TtSM2)",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
            });
        }
    }
}