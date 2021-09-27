using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

using SmartSchoolBellCore.Extension;
using SmartSchoolBellCore.Model;
using SmartSchoolBellCore.View;
using static SmartSchoolBellCore.Services.StartBellService;

namespace SmartSchoolBellCore.ViewModel
{
    public class TimetablePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDialogOpen { get; set; }
        public object DialogContent { get; set; }

        public TimetablePageViewModel(int timetableId)
        {
            TimetableId = timetableId;
            MenuNavigationChoice("Пн.");
        }

        public bool IsWorking
        {
            get
            {
                using var context = new DatabaseContext();
                return context.Timetables.Find(TimetableId).Working;
            }

            set
            {
                using (var context = new DatabaseContext())
                {
                    context.Timetables.Find(TimetableId).Working = value;
                    context.SaveChanges();
                }
                OnPropertyChanged(nameof(IsWorking));
                StartTimerBell();
            }
        }

        public TimetablePageViewModel(){}

        private int TimetableId { get; }

        public string TextUriFile
        {
            get
            {
                using var context = new DatabaseContext();
                return context.Timetables.Find(TimetableId).UriFile;
            }
            set
            {
                using (var context = new DatabaseContext())
                {
                    History.GetToDatabaseAsync(context, new History(DateTime.Now, "Файл звука звонка изменён с " + TextUriFile + " на " + value
                    + " в " + context.Timetables.Find(TimetableId).Name));
                    context.Timetables.Find(TimetableId).UriFile = value;
                    context.SaveChanges();           
                }
                StartTimerBell();
            }
        }


        private void LoadingItemsTimeBell(TimetableDayOfWeek timetableDayOfWeek)
        {
            ItemsTimeBell.Clear();
            using var context = new DatabaseContext();
            var sortTimeBells = context.TimeHourMins.Include(i => i.TimetableDayOfWeek.TimeBells)
                .Where(i => i.TimetableDayOfWeek.Id == timetableDayOfWeek.Id)
                .OrderBy(i => i.Hour * 60 + i.Min).ToList();

            for (var i = 0; i < sortTimeBells.Count; i++)
            {
                var item = new ItemCallTimesViewModel(new(0, sortTimeBells[i].Hour * 60 + sortTimeBells[i].Min, 0),i);
                var i2 = i;
                item.DialogDeleteEvent += listId =>
                {
                    var dialog = new DialogDeleteViewModel();
                    dialog.EventClickCancelButton += () =>
                    {
                        IsDialogOpen = false;
                        OnPropertyChanged(nameof(IsDialogOpen));
                    };

                    var i1 = i2;
                    dialog.EventClickYesButton += () =>
                    {
                        DeleteTimeBell(sortTimeBells[i1].Id);
                        IsDialogOpen = false;
                        OnPropertyChanged(nameof(IsDialogOpen));
                    };

                    DialogContent = new DialogDelete(dialog);
                    OnPropertyChanged(nameof(DialogContent));
                    IsDialogOpen = true;
                    OnPropertyChanged(nameof(IsDialogOpen));
                };
                ItemsTimeBell.Add(item);
            }

            var addItem = new ItemCallTimesViewModel(true)
            {
                EventDialogNewTimeBell = () =>
                {
                    var dialog = new DialogNewTimeBellViewModel
                    {
                        EventCancel = () =>
                        {
                            IsDialogOpen = false;
                            OnPropertyChanged(nameof(IsDialogOpen));
                        },
                        EventNewTimeBell = (hour,min) =>
                        {
                            NewTimeBell(hour,min, timetableDayOfWeek.Id);
                            IsDialogOpen = false;
                            OnPropertyChanged(nameof(IsDialogOpen));
                        }
                    };
                    DialogContent = new DialogNewTimeBell(dialog);
                    OnPropertyChanged(nameof(DialogContent));
                    IsDialogOpen = true;
                    OnPropertyChanged(nameof(IsDialogOpen));
                }
            };

            ItemsTimeBell.Add(addItem); 
            }

        private async Task NewTimeBell(int hour,int min, int timetableDayOfWeekId)
        {
            var hourMin = new TimeHourMin(hour, min);

            await using var context = new DatabaseContext();

            var isEqual = context.TimetableDayOfWeeks.Where(i => 
                    context.TimetableDayOfWeeks.Find(timetableDayOfWeekId).DayOfWeekNumber == i.DayOfWeekNumber)
                .Any(i => i.TimeBells.Any(t => t.Hour * 60 + t.Min == hourMin.Hour * 60 + hourMin.Min));

            if (isEqual)
                MessageBox.Show("Звонок на такое время уже существует", "Ошибка создания звонка", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                var timeHourMin = new TimeHourMin(hour, min)
                {
                    TimetableDayOfWeekId = timetableDayOfWeekId
                };
                await context.TimeHourMins.AddAsync(timeHourMin);
                await context.SaveChangesAsync();

                StartTimerBell();
                MenuNavigationChoice(_menuNavigationDayOfWeek);

                await History.GetToDatabaseAsync(context, new History(DateTime.Now, "Звонок на "
                                                                + new TimeSpan(hour, min, 0).ToString("HH:mm") +
                                                                " создан в расписании " +
                                                                (await context.Timetables.FindAsync(TimetableId)).Name));

            }
        }

        private async Task DeleteTimeBell(int timeBellId)
        {
            await using (var context = new DatabaseContext())
{
                var name = (await context.Timetables.FindAsync(TimetableId)).Name;
                var timeBell = await context.TimeHourMins.Include(i => i.TimetableDayOfWeek).FirstAsync(i => i.Id == timeBellId);
                context.TimeHourMins.Remove(timeBell);
                await context.SaveChangesAsync();
                await History.GetToDatabaseAsync(context, new History(DateTime.Now,
                    "Звонок на " + new TimeSpan(timeBell.Hour, timeBell.Min, 0).ToString(@"hh\:mm") +
                    " удалён из расписания " +
                    name));
            }
            StartTimerBell();
            MenuNavigationChoice(_menuNavigationDayOfWeek);
        }

        public ICommand ComOpenDialogFile => new DelegateCommand(o =>
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All Supported Audio | *.mp3; *.wav; | MP3 | *.mp3; | WAV | *.wav;"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                TextUriFile = openFileDialog.FileName;
                OnPropertyChanged(nameof(TextUriFile));
            }
        });

        public ObservableCollection<ItemCallTimesViewModel> ItemsTimeBell { get; set; } = new();

        private string _menuNavigationDayOfWeek;
        private void MenuNavigationChoice(string dayOfWeak)
        {
            _menuNavigationDayOfWeek = dayOfWeak;

            using var context = new DatabaseContext();

            LoadingItemsTimeBell(dayOfWeak.GetTimetableDayOfWeekAsync(context, TimetableId).Result);
        }

        public ICommand ComDeleteMenuNavigation => new DelegateCommand(o=> 
        {
            using (var context = new DatabaseContext())
            {
                var bells = 
                    context.TimeHourMins.Where(i => i.TimetableDayOfWeekId == ((string)o).GetTimetableDayOfWeekAsync(context, TimetableId).Result.Id);
                foreach (var bell in bells)
                    context.Remove(bell);
                context.SaveChanges();
                History.GetToDatabaseAsync(context, new History(DateTime.Now, (string)o + " из расписании " + context.Timetables.Find(TimetableId).Name+" очищен(а)"));
            }
            
            MenuNavigationChoice(_menuNavigationDayOfWeek);
        });

        public ICommand ComImportMenuNavigation => new DelegateCommand(o =>
        {
            using (var context = new DatabaseContext())
            {
                var import =
                    context.TimeHourMins.Where(i =>
                        i.TimetableDayOfWeekId == ((string) o).GetTimetableDayOfWeekAsync(context, TimetableId)
                        .Result.Id).ToList();

                var id = _menuNavigationDayOfWeek.GetTimetableDayOfWeekAsync(context, TimetableId).Result.Id;

                foreach (var bell in import)
                {
                    var bellNew = new TimeHourMin(bell.Hour, bell.Min)
                    {
                        TimetableDayOfWeekId = id
                    };
                    context.Add(bellNew);
                }

                context.SaveChanges();

                History.GetToDatabaseAsync(context, new History(DateTime.Now,
                    "В расписании " + context.Timetables.Find(TimetableId).Name + " импорт с " +
                    (string) o + " на " + _menuNavigationDayOfWeek));
            }

            MenuNavigationChoice(_menuNavigationDayOfWeek);

        });

        public ICommand MenuNavigation => new DelegateCommand(o => MenuNavigationChoice((string) o));
    }
}