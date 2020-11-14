using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Smart_school_bell.Annotations;
using Smart_school_bell.Model;
using Smart_school_bell.View;

namespace Smart_school_bell.ViewModel
{
    public class SchedulesPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<MainWindowNavigationItemViewModel> NavigationItemsItemsControl { get; set; }
            = new ObservableCollection<MainWindowNavigationItemViewModel>();


        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged("IsDialogOpen");
            }
        }

        private object _dialogContent;
        public object DialogContent
        {
            get { return _dialogContent; }
            set
            {
                if (_dialogContent == value) return;
                _dialogContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand ComNewTimetable
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    DialogNewTimetableViewModel dialog = new DialogNewTimetableViewModel();
                    dialog.EventCancel += () => IsDialogOpen = false;
                    dialog.EventNewItem += s =>
                    {

                        NewTimetable(s);
                        IsDialogOpen = false;
                    };
                    DialogContent = new DialogNewTimetable(dialog);
                    IsDialogOpen = true;
                });
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SchedulesPageViewModel()
        {

            LoadingTimetables();
            Timetable.StartTimerBell();
        }

        private void NewTimetable(string name)
        {

            using (var context = new DatabaseContext())
            {
                context.Timetables.Add(new Timetable(name));
                context.SaveChanges();

                context.Timetables.Find(context.Timetables.Count()).Monday.TimetableId
                    = context.Timetables.Find(context.Timetables.Count()).Id;
                context.Timetables.Find(context.Timetables.Count()).Tuesday.TimetableId
                    = context.Timetables.Find(context.Timetables.Count()).Id;
                context.Timetables.Find(context.Timetables.Count()).Wednesday.TimetableId
                    = context.Timetables.Find(context.Timetables.Count()).Id;
                context.Timetables.Find(context.Timetables.Count()).Thursday.TimetableId
                    = context.Timetables.Find(context.Timetables.Count()).Id;
                context.Timetables.Find(context.Timetables.Count()).Friday.TimetableId
                    = context.Timetables.Find(context.Timetables.Count()).Id;
                context.Timetables.Find(context.Timetables.Count()).Saturday.TimetableId
                    = context.Timetables.Find(context.Timetables.Count()).Id;
                History.GetToDatabase(new History(DateTime.Now, "Создано расписание " + name));
                context.SaveChanges();
                
            }
            LoadingTimetables();
        }

        public void LoadingTimetables()
        {
            NavigationItemsItemsControl.Clear();
            using (var context = new DatabaseContext())
            {
                var timetablesSort
                    = context.Timetables.OrderBy(t => t.Name);
                foreach (var itemTimetable in timetablesSort)
                {
                    MainWindowNavigationItemViewModel mainWindowNavigationItemViewModel
                        = new MainWindowNavigationItemViewModel(itemTimetable);

                    mainWindowNavigationItemViewModel.EventDeleteItem += () =>
                    {
                        DialogDeleteViewModel dialog = new DialogDeleteViewModel();
                        dialog.EventClickCancelButton += () => IsDialogOpen = false;
                        dialog.EventClickYesButton += () =>
                        {
                            DeleteTimetable(itemTimetable.Id);
                            IsDialogOpen = false;
                        };
                        DialogContent = new DialogDelete(dialog);
                        IsDialogOpen = true;
                    };

                    mainWindowNavigationItemViewModel.EventRenameTimetable
                        += () =>
                        {
                            DialogRenameTimetableVM dialog = new DialogRenameTimetableVM(itemTimetable.Name);
                            dialog.EventCancel += () =>
                            {
                                IsDialogOpen = false;
                                OnPropertyChanged("IsDialogOpen");
                            };
                            dialog.EventRename += (name) =>
                            {
                                IsDialogOpen = false;
                                OnPropertyChanged("IsDialogOpen");
                                RenameTimetable(itemTimetable.Id, name);
                            };
                            DialogContent = new DialogRenameTimetable(dialog);
                            IsDialogOpen = true;
                            OnPropertyChanged("IsDialogOpen");
                        };

                    mainWindowNavigationItemViewModel.EventClickTimetable += () => ClickTimetable(itemTimetable);
                    NavigationItemsItemsControl
                    .Add(mainWindowNavigationItemViewModel);
                    OnPropertyChanged("NavigationItemsItemsControl");
                }
            }
        }

        private void ClickTimetable(Timetable timetable)
        {
            TimetablePage = new TimetablePage(new TimetablePageVM(timetable.Id));
            OnPropertyChanged("TimetablePage");
        }
        private void DeleteTimetable(int id)
        {
            using (var context = new DatabaseContext())
            {

                Timetable timetable = context.Timetables.Find(id);

                context.TimetableDayOfWeeks.Remove(timetable.Monday);
                context.TimetableDayOfWeeks.Remove(timetable.Tuesday);
                context.TimetableDayOfWeeks.Remove(timetable.Wednesday);
                context.TimetableDayOfWeeks.Remove(timetable.Thursday);
                context.TimetableDayOfWeeks.Remove(timetable.Friday);
                context.TimetableDayOfWeeks.Remove(timetable.Saturday);
                context.SaveChanges();

                context.Timetables.Remove(context.Timetables.Find(id));
                History.GetToDatabase(new History(DateTime.Now, "Удалено расписание " + timetable.Name));
                context.SaveChanges();
                
            }
            Timetable.StartTimerBell();
            LoadingTimetables();
        }

        private void RenameTimetable(int id, string name)
        {
            using (var context = new DatabaseContext())
            {
                History.GetToDatabase(new History(DateTime.Now, "Расписание переименовано с "
                                                                + context.Timetables.Find(id).Name +
                                                                " на " + name));

                context.Timetables.Find(id).Name = name;
                context.SaveChanges();
            }
            
            LoadingTimetables();
        }





        public Page TimetablePage { get; set; }
    }
}