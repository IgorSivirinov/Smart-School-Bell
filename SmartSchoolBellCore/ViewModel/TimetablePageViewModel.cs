using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

using SmartSchoolBellCore.Extension;
using SmartSchoolBellCore.Model;
using SmartSchoolBellCore.View;
using static SmartSchoolBellCore.Services.StartBellService;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace SmartSchoolBellCore.ViewModel
{
    public class TimetablePageViewModel : INotifyPropertyChanged
    {
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
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
                Task.Run(async () =>  await StartTimerBell(_dispatcher));
            }
        }

        public TimetablePageViewModel(){}

        private int TimetableId { get; }

        private string _textUriFile = null;

        public string TextUriFile
        {
            get
            {
                if(_textUriFile == null)
                {
                    using var context = new DatabaseContext();
                    _textUriFile = context.Timetables.Find(TimetableId).UriFile;
                }
                return _textUriFile;
            }
            set
            {
                var current = _textUriFile;
                _textUriFile = value;
                OnPropertyChanged(nameof(TextUriFile));

                Task.Run(async () => { 
                    await using var context = new DatabaseContext();
                    await History.GetToDatabaseAsync(context, new History(DateTime.Now, "Файл звука звонка изменён с " +
                        current + " на " + value
                        + " в " + context.Timetables.Find(TimetableId).Name));
                    context.Timetables.Find(TimetableId).UriFile = value;
                    context.SaveChanges();
                    await StartTimerBell(_dispatcher);
                });
            }
        }


        private void LoadingItemsTimeBellAsync(string dayOfWeak)
        {
            _dispatcher.BeginInvoke(() =>
            {
                using var context = new DatabaseContext();

                var timetableDayOfWeek = dayOfWeak.GetTimetableDayOfWeekAsync(context, TimetableId).Result;
                var sortTimeBells = context.TimeHourMins.Include(i => i.TimetableDayOfWeek.TimeBells)
                    .Where(i => i.TimetableDayOfWeek.Id == timetableDayOfWeek.Id)
                    .OrderBy(i => i.Hour * 60 + i.Min).ToList();

                ListTimeBell = sortTimeBells;

                ItemsTimeBell.Clear();
                for (var i = 0; i < sortTimeBells.Count; i++)
                {

                    var item = new ItemCallTimesViewModel(new(0, sortTimeBells[i].Hour * 60 + sortTimeBells[i].Min, 0), i);
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
                            Task.Run(async () => await DeleteTimeBell(sortTimeBells[i1].Id));
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
                            EventNewTimeBell = (hour, min) =>
                            {
                                Task.Run(async () => await NewTimeBell(hour, min, timetableDayOfWeek.Id));
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
            });
           
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


                MenuNavigationChoice(_menuNavigationDayOfWeek);

                await History.GetToDatabaseAsync(context, new History(DateTime.Now, "Звонок на "
                                + new TimeSpan(hour, min, 0).ToString(@"hh\:mm") +
                                " создан в расписании " +
                                (await context.Timetables.FirstAsync(x => x.Id == TimetableId)).Name));

                await Task.Run(async () => await StartTimerBell(_dispatcher));

                //ListTimeBell.Add(timeHourMin);
                //var index = ListTimeBell.OrderBy(x => x.Hour * 60 + x.Min).ToList().IndexOf(timeHourMin);

                //ItemsTimeBell.Insert(index, new(new(timeHourMin.Hour, timeHourMin.Min, 0), index));
                


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

            MenuNavigationChoice(_menuNavigationDayOfWeek);

            await Task.Run(async () => await StartTimerBell(_dispatcher));
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
            }
        });

        public ObservableCollection<ItemCallTimesViewModel> ItemsTimeBell { get; set; } = new();
        public List<TimeHourMin> ListTimeBell { get; set; } = new();

        private string _menuNavigationDayOfWeek;
        private void MenuNavigationChoice(string dayOfWeak)
        {
            _dispatcher.BeginInvoke(() =>
            {
                _menuNavigationDayOfWeek = dayOfWeak;

                

                
                LoadingItemsTimeBellAsync(dayOfWeak);
            });
        }

        public ICommand ComDeleteMenuNavigation => new DelegateCommand(o =>
        {
            Task.Run(async () => 
                {
                    await using var context = new DatabaseContext();
            
                    var bells =
                        context.TimeHourMins.Where(i =>
                            i.TimetableDayOfWeekId ==
                            ((string) o).GetTimetableDayOfWeekAsync(context, TimetableId).Result.Id);
                    foreach (var bell in bells)
                        context.Remove(bell);

                    context.SaveChanges();
                    await History.GetToDatabaseAsync(context,
                        new History(DateTime.Now,
                            (string) o + " из расписании " + context.Timetables.Find(TimetableId).Name + " очищен(а)"));
                    MenuNavigationChoice(_menuNavigationDayOfWeek);
                    await StartTimerBell(_dispatcher);
                });    
        });

        public ICommand ComImportMenuNavigation => new DelegateCommand(o =>
        {
            Task.Run(async () =>
            {
                await using var context = new DatabaseContext();
                var import =
                    context.TimeHourMins.Where(i =>
                        i.TimetableDayOfWeekId == ((string) o).GetTimetableDayOfWeekAsync(context, TimetableId)
                        .Result.Id).ToList();

                var id = (await _menuNavigationDayOfWeek.GetTimetableDayOfWeekAsync(context, TimetableId)).Id;

                var bells =
                    context.TimeHourMins.Where(i =>
                        i.TimetableDayOfWeekId == id);
                foreach (var bell in bells)
                    context.Remove(bell);

                foreach (var bellNew in import.Select(bell => new TimeHourMin(bell.Hour, bell.Min)
                         {
                             TimetableDayOfWeekId = id
                         }))
                {
                    context.TimeHourMins.Add(bellNew);
                }

                context.SaveChanges();

                MenuNavigationChoice(_menuNavigationDayOfWeek);

                await History.GetToDatabaseAsync(context, new History(DateTime.Now,
                    "В расписании " + context.Timetables.Find(TimetableId).Name + " импорт с " +
                    (string) o + " на " + _menuNavigationDayOfWeek));

                await StartTimerBell(_dispatcher);
            });
        });

        public ICommand MenuNavigation => new DelegateCommand(o => MenuNavigationChoice((string) o));
    }
}