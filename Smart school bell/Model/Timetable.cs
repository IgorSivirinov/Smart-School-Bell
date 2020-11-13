using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace Smart_school_bell.Model
{
    public class Timetable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UriFile { get; set; }
        public int СallDuration { get; set; }

        public virtual TimetableDayOfWeek Monday { get; set; }
        public virtual TimetableDayOfWeek Tuesday { get; set; }
        public virtual TimetableDayOfWeek Wednesday { get; set; }
        public virtual TimetableDayOfWeek Thursday { get; set; }
        public virtual TimetableDayOfWeek Friday { get; set; }
        public virtual TimetableDayOfWeek Saturday { get; set; }
        public Timetable(string name)
        {
            Name = name;

            Monday = new TimetableDayOfWeek(DayOfWeek.Monday);
            Tuesday = new TimetableDayOfWeek(DayOfWeek.Tuesday);
            Wednesday = new TimetableDayOfWeek(DayOfWeek.Wednesday);
            Thursday = new TimetableDayOfWeek(DayOfWeek.Thursday);
            Friday = new TimetableDayOfWeek(DayOfWeek.Friday);
            Saturday = new TimetableDayOfWeek(DayOfWeek.Saturday);
        }

        public Timetable(){}

        private static Thread thiredStartMusic = new Thread(() => StartMusic());
        private static Thread thiredResetTimer= new Thread(() => ResetTimer());
        private static Timer TimerBell = new Timer();
        private static Timer TimerDayOfWeek = new Timer();
        private static List<TimetableDayOfWeek.TimeHourMin> times = new List<TimetableDayOfWeek.TimeHourMin>();
        public static void StartTimerBell()
        {
            times.Clear();
            TimeSpan timeSpanNow = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

            using (var context = new DatabaseContext())
            {

                foreach (TimetableDayOfWeek timetableDayOfWeek in context.TimetableDayOfWeeks)
                {


                    if (timetableDayOfWeek.DayOfWeek == DateTime.Now.DayOfWeek)
                    {
                        List<TimetableDayOfWeek.TimeHourMin> sortTimeBells = timetableDayOfWeek.TimeBells;
                        sortTimeBells.Sort(new SortTimeHourMinModel());

                        foreach (TimetableDayOfWeek.TimeHourMin time in sortTimeBells)
                        {
                            if (time.Time > timeSpanNow)
                            {
                                using (var context2 = new DatabaseContext())
                                {
                                    time.Uri = context2.Timetables.Find(timetableDayOfWeek.TimetableId).UriFile;
                                    time.СallDuration = context2.Timetables.Find(timetableDayOfWeek.TimetableId).СallDuration;
                                    times.Add(time);
                                }
                                break;
                            }

                        }
                    }
                }
            }

            if (times.Count != 0)
            {
                TimerBell.Elapsed -= TimerBell_Elapsed;
                TimerDayOfWeek.Elapsed -= TimerDayOfWeek_Elapsed;

                times.Sort(new SortTimeHourMinModel());
                TimerBell.Interval =
                    1000 * (times[0].Time - new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))
                    .TotalSeconds;
                
                    TimerBell.Elapsed += TimerBell_Elapsed;
                    TimerBell.AutoReset = false;
                    TimerBell.Enabled = true;
                    TimerBell.Start();

                    try
                    {
                        TimerDayOfWeek.Interval
                            = 1000 * new DateTime(DateTime.Now.Year,
                                    DateTime.Now.Month,
                                    DateTime.Now.Day + 1, 0, 0, 0)
                                .Subtract(DateTime.Now)
                                .TotalSeconds;
                    }
                    catch (Exception)
                    {
                        TimerDayOfWeek.Interval
                            = 1000 * new DateTime(DateTime.Now.Year,
                                    DateTime.Now.Month + 1,
                                    1, 0, 0, 0)
                                .Subtract(DateTime.Now)
                                .TotalSeconds;
                    }
                TimerDayOfWeek.Elapsed += TimerDayOfWeek_Elapsed;
                TimerDayOfWeek.AutoReset = false;
                TimerDayOfWeek.Enabled = true;
                TimerDayOfWeek.Start();
            } 
        }

        private static void TimerDayOfWeek_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            thiredResetTimer = new Thread(() => ResetTimer());
            thiredResetTimer.Start();
        }

        private static void TimerBell_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            thiredStartMusic = new Thread(() => StartMusic());
            thiredStartMusic.Start();
        }

        private static void ResetTimer()
        {
            TimerDayOfWeek.Stop();
            StartTimerBell();
        }
        private static void StartMusic()
        {
            MediaPlayer player = new MediaPlayer();
            if (times[0].Uri != null)
            {
                player.Open(new Uri(times[0].Uri, UriKind.Absolute));
                player.Play();
                int timerErrorPlay = 0;
                while (true)
                {
                    Thread.Sleep(1000);
                    timerErrorPlay++;
                    if (!player.HasAudio)
                    {
                        if (timerErrorPlay >= 20)
                        {
                            MessageBox.Show("Проверьте расположение файла", "Ошибка воспроизведения файла (TtSM1)", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }
                    }
                    else
                    {

                        int timer = 0;
                        while (true)
                        {
                            Thread.Sleep(1000);
                            timer++;
                            if (timer == times[0].СallDuration)
                            {
                                player.Close();
                                break;
                            }

                            if (player.Position.TotalSeconds == player.NaturalDuration.TimeSpan.TotalSeconds)
                            {
                                player.Position = new TimeSpan(0, 0, 0);
                                player.Play();
                            }

                        }
                        break;
                    }
                    //}
                    //catch (Exception)
                    //{
                    //    MessageBox.Show("Измените расположение файла", "Ошибка загрузки файла", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                }
                    StartTimerBell();
                }
                else
                {
                    MessageBox.Show("Проверьте расположение файла", "Ошибка воспроизведения файла (TtSM2)", MessageBoxButton.OK, MessageBoxImage.Error);
                    StartTimerBell();
                }
            


           
            
        }
    }
}