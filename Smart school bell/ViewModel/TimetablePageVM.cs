using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using Smart_school_bell.Annotations;
using Smart_school_bell.Model;
using Smart_school_bell.View;

namespace Smart_school_bell.ViewModel
{
    public class TimetablePageVM : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDialogOpen { get; set; }
        public object DialogContent { get; set; }

        public TimetablePageVM(int timetableId)
        {
            TimetableId = timetableId;
            MenuNavigationChoice("Пн.");
        }

        public TimetablePageVM(){}

        private int TimetableId { get; set; }

        public string TextUriFile
        {
            get
            {
                using (var context = new DatabaseContext())
                {
                    return context.Timetables.Find(TimetableId).UriFile;
                }
            }
            set
            {
                using (var context = new DatabaseContext())
                {
                    context.Timetables.Find(TimetableId).UriFile = value;
                    context.SaveChanges();           
                }
                Timetable.StartTimerBell();
            }
        }

        public string СallDuration {
            get
            {
                using (var context = new DatabaseContext())
                {
                    return context.Timetables.Find(TimetableId).СallDuration.ToString();
                }
            }
            set
            {
                using (var context = new DatabaseContext())
                {
                    int Num;
                    if (int.TryParse(value, out Num))
                        context.Timetables.Find(TimetableId).СallDuration = int.Parse(value);

                    if (string.IsNullOrWhiteSpace(value))
                        context.Timetables.Find(TimetableId).СallDuration = 0;

                    context.SaveChanges();
                }
                Timetable.StartTimerBell();
            }
        }

        private void LoadingItemsTimeBell(TimetableDayOfWeek timetableDayOfWeek)
        {
            ItemsTimeBell.Clear();

            List<TimetableDayOfWeek.TimeHourMin> sortTimeBells= timetableDayOfWeek.TimeBells;
            sortTimeBells.Sort(new SortTimeHourMinModel());

            for (int i = 0; i < sortTimeBells.Count;i++)
            {
                ItemCallTimesViewModel item = new ItemCallTimesViewModel(sortTimeBells[i].Time,i);
                item.DialogDeleteEvent += listId =>
                {
                    DialogDeleteViewModel dialog = new DialogDeleteViewModel();
                    dialog.EventClickCancelButton += () =>
                    {
                        IsDialogOpen = false;
                        OnPropertyChanged("IsDialogOpen");
                    };

                   
                    dialog.EventClickYesButton += () =>
                    {
                        DeleteTimeBell(timetableDayOfWeek.Id, listId);
                        IsDialogOpen = false;
                        OnPropertyChanged("IsDialogOpen");
                        Timetable.StartTimerBell();
                    };

                    DialogContent = new DialogDelete(dialog);
                    OnPropertyChanged("DialogContent");
                    IsDialogOpen = true;
                    OnPropertyChanged("IsDialogOpen");
                };
                ItemsTimeBell
                    .Add(item);
            }
                
                
                
            ItemCallTimesViewModel addItem = new ItemCallTimesViewModel(true);
            addItem.EventDialogNewTimeBell += () =>
            {
                DialogNewTimeBellVM dialog = new DialogNewTimeBellVM();
                dialog.EventCancel += () =>
                {
                    IsDialogOpen = false;
                    OnPropertyChanged("IsDialogOpen");
                };
                dialog.EventNewTimeBall += (hour,min) =>
                {
                    NewTimeBell(hour,min, timetableDayOfWeek.Id);
                    IsDialogOpen = false;
                    OnPropertyChanged("IsDialogOpen");
                };
                DialogContent = new DialogNewTimeBell(dialog);
                OnPropertyChanged("DialogContent");
                IsDialogOpen = true;
                OnPropertyChanged("IsDialogOpen");
            };
            ItemsTimeBell.Add(addItem); 
            }

        private void NewTimeBell(int hour,int min, int timetableDayOfWeekId)
        {
            bool isEqual = false;
            using (var context = new DatabaseContext())
            {
                foreach (TimetableDayOfWeek timetableDayOfWeek in context.TimetableDayOfWeeks)
                {

                    using (var context2 = new DatabaseContext())
                    {
                        if (context2.TimetableDayOfWeeks.Find(timetableDayOfWeekId).DayOfWeek == timetableDayOfWeek.DayOfWeek)
                        {
                            foreach (var timeBell in timetableDayOfWeek.TimeBells)
                                if (hour == timeBell.Hour && min == timeBell.Min)
                                    isEqual = true;
                        }
                    }
                }

                if (isEqual)
                    MessageBox.Show("Звонок на такое время уже существует", "Ошибка создания звонка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    List<TimetableDayOfWeek.TimeHourMin> timeBels = context.TimetableDayOfWeeks.Find(timetableDayOfWeekId).TimeBells;
                    timeBels.Add(new TimetableDayOfWeek.TimeHourMin(hour, min));
                    context.TimetableDayOfWeeks.Find(timetableDayOfWeekId).TimeBells = timeBels;

                    context.SaveChanges();
                    Timetable.StartTimerBell();
                    MenuNavigationChoice(menuNavigationDayOfWeek);
                }
            }

        }

            private void DeleteTimeBell(int timetableDayOfWeekId, int listId)
        {
            using (var context = new DatabaseContext())
            {
                List<TimetableDayOfWeek.TimeHourMin> timeBels = context.TimetableDayOfWeeks.Find(timetableDayOfWeekId).TimeBells;
                timeBels.Sort(new SortTimeHourMinModel());
                timeBels.RemoveAt(listId);
                context.TimetableDayOfWeeks.Find(timetableDayOfWeekId).TimeBells = timeBels;
                context.SaveChanges();
            }
            Timetable.StartTimerBell();
            MenuNavigationChoice(menuNavigationDayOfWeek);
        }





        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "СallDuration":
                        if (string.IsNullOrWhiteSpace(СallDuration))
                        {
                            result = "Пустое поле";
                        }
                        break;
                }

                return result;
            }
        }

        public string Error { get; }

        public ICommand ComOpenDialogFile
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "All Supported Audio | *.mp3; *.wav; | MP3 | *.mp3; | WAV | *.wav;";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        TextUriFile = openFileDialog.FileName;
                        OnPropertyChanged("TextUriFile");
                    }
                });
            }
        }

        public ObservableCollection<ItemCallTimesViewModel> ItemsTimeBell { get; set; }
            = new ObservableCollection<ItemCallTimesViewModel>();



        public string menuNavigationDayOfWeek;
        private void MenuNavigationChoice(string dayOfWeak)
        {
            menuNavigationDayOfWeek = dayOfWeak;
            using (var context = new DatabaseContext())
            {
                switch (dayOfWeak)
                {
                    case "Пн.":
                        LoadingItemsTimeBell(context.Timetables.Find(TimetableId).Monday);
                        break;

                    case "Вт.":
                        LoadingItemsTimeBell(context.Timetables.Find(TimetableId).Tuesday);
                        break;

                    case "Ср.":
                        LoadingItemsTimeBell(context.Timetables.Find(TimetableId).Wednesday);
                        break;

                    case "Чт.":
                        LoadingItemsTimeBell(context.Timetables.Find(TimetableId).Thursday);
                        break;

                    case "Пт.":
                        LoadingItemsTimeBell(context.Timetables.Find(TimetableId).Friday);
                        break;

                    case "Су.":
                        LoadingItemsTimeBell(context.Timetables.Find(TimetableId).Saturday);
                        break;
                }
            }
        }

        public string importOutJson;

        private void DeleteData()
        {
        }
        private void ImportData()
        {
            using (var context = new DatabaseContext())
            {
                switch (menuNavigationDayOfWeek)
                {
                    case "Пн.":
                        context.Timetables.Find(TimetableId).Monday.TimeBellJSON = importOutJson;
                        break;

                    case "Вт.":
                        context.Timetables.Find(TimetableId).Tuesday.TimeBellJSON = importOutJson;
                        break;

                    case "Ср.":
                        context.Timetables.Find(TimetableId).Wednesday.TimeBellJSON = importOutJson;
                        break;

                    case "Чт.":
                        context.Timetables.Find(TimetableId).Thursday.TimeBellJSON = importOutJson;
                        break;

                    case "Пт.":
                        context.Timetables.Find(TimetableId).Friday.TimeBellJSON = importOutJson;
                        break;

                    case "Су.":
                        context.Timetables.Find(TimetableId).Saturday.TimeBellJSON = importOutJson;
                        break;
                }
                context.SaveChanges();
            }
            MenuNavigationChoice(menuNavigationDayOfWeek);
        }

        public ICommand ComDeleteMenuNavigation
        {
            get
            {
                return new DelegateCommand(o=>
                {

                    using (var context = new DatabaseContext())
                    {
                        switch ((string)o)
                        {
                            case "Пн.":
                                context.Timetables.Find(TimetableId).Monday.TimeBellJSON = "[]";
                                break;

                            case "Вт.":
                                context.Timetables.Find(TimetableId).Tuesday.TimeBellJSON = "[]";
                                break;

                            case "Ср.":
                                context.Timetables.Find(TimetableId).Wednesday.TimeBellJSON = "[]";
                                break;

                            case "Чт.":
                                context.Timetables.Find(TimetableId).Thursday.TimeBellJSON = "[]";
                                break;

                            case "Пт.":
                                context.Timetables.Find(TimetableId).Friday.TimeBellJSON = "[]";
                                break;

                            case "Су.":
                                context.Timetables.Find(TimetableId).Saturday.TimeBellJSON = "[]";
                                break;
                        }
                        context.SaveChanges();
                    }
                    MenuNavigationChoice(menuNavigationDayOfWeek);
                });
            }
        }
        public ICommand ComImportMenuNavigation
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    using (var context = new DatabaseContext())
                    {
                        switch ((string)o)
                        {
                            case "Пн.":
                                importOutJson = context.Timetables.Find(TimetableId).Monday.TimeBellJSON;
                                break;

                            case "Вт.":
                                importOutJson = context.Timetables.Find(TimetableId).Tuesday.TimeBellJSON;
                                break;

                            case "Ср.":
                                importOutJson = context.Timetables.Find(TimetableId).Wednesday.TimeBellJSON;
                                break;

                            case "Чт.":
                                importOutJson = context.Timetables.Find(TimetableId).Thursday.TimeBellJSON;
                                break;

                            case "Пт.":
                                importOutJson = context.Timetables.Find(TimetableId).Friday.TimeBellJSON;
                                break;

                            case "Су.":
                                importOutJson = context.Timetables.Find(TimetableId).Saturday.TimeBellJSON;
                                break;
                        }
                    }
                    ImportData();

                });

            }
        }

        public ICommand MenuNavigation
        {
            get { return new DelegateCommand(o => MenuNavigationChoice((string) o)); }
        }
    }
}