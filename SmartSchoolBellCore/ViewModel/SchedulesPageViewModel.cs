using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using SmartSchoolBellCore.Model;
using SmartSchoolBellCore.View;
using static SmartSchoolBellCore.Services.StartBellService;

namespace SmartSchoolBellCore.ViewModel
{
    public class SchedulesPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<MainWindowNavigationItemViewModel> NavigationItemsItemsControl { get; set; }
            = new ObservableCollection<MainWindowNavigationItemViewModel>();


        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged(nameof(IsDialogOpen));
            }
        }

        private object _dialogContent;
        public object DialogContent
        {
            get => _dialogContent;
            set
            {
                if (_dialogContent == value) return;
                _dialogContent = value;
                OnPropertyChanged(nameof(DialogContent));
            }
        }

        public ICommand ComNewTimetable
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    var dialog = new DialogNewTimetableViewModel();
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SchedulesPageViewModel()
        {
            LoadingTimetables();
            StartTimerBell();
        }

        private void NewTimetable(string name)
        {

            using (var context = new DatabaseContext())
            {
                var timetable = new Timetable(name);
                context.Timetables.Add(timetable);

                context.SaveChanges();

                for(var i = 0; i < 6; i++)
                    context.Timetables.Find(timetable.Id).TimetableDayOfWeeks[i].TimetableId
                        = context.Timetables.Find(timetable.Id).Id;

                History.GetToDatabaseAsync(context, new History(DateTime.Now, "Создано расписание " + name));

                context.SaveChanges();
                
            }
            LoadingTimetables();
        }

        public void LoadingTimetables()
        {
            NavigationItemsItemsControl.Clear();
            using var context = new DatabaseContext();
            var timetablesSort
                = context.Timetables.OrderBy(t => t.Name);
            foreach (var itemTimetable in timetablesSort)
            {
                var mainWindowNavigationItemViewModel
                    = new MainWindowNavigationItemViewModel(itemTimetable);

                mainWindowNavigationItemViewModel.EventDeleteItem += () =>
                {
                    var dialog = new DialogDeleteViewModel();
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
                        var dialog = new DialogRenameTimetableViewModel(itemTimetable.Name);
                        dialog.EventCancel += () => IsDialogOpen = false;
                        ;
                        dialog.EventRename += name =>
                        {
                            RenameTimetable(itemTimetable.Id, name);
                            IsDialogOpen = false;
                        };
                        DialogContent = new DialogRenameTimetable(dialog);
                        IsDialogOpen = true;
                    };

                mainWindowNavigationItemViewModel.EventClickTimetable += () => ClickTimetable(itemTimetable);
                NavigationItemsItemsControl
                    .Add(mainWindowNavigationItemViewModel);
                OnPropertyChanged(nameof(NavigationItemsItemsControl));
            }
        }

        private void ClickTimetable(Timetable timetable)
        {
            TimetablePage = new TimetablePage(new TimetablePageViewModel(timetable.Id));
            OnPropertyChanged(nameof(TimetablePage));
        }

        private void DeleteTimetable(int id)
        {
            using (var context = new DatabaseContext())
            {

                var timetable = context.Timetables.Find(id);

                context.Timetables.Remove(timetable);

                History.GetToDatabaseAsync(context, new History(DateTime.Now, "Удалено расписание " + timetable.Name));

                context.SaveChanges();
            }
            StartTimerBell();
            App.RestartMainWindow();
        }

        private void RenameTimetable(int id, string name)
        {
            using (var context = new DatabaseContext())
            {
                History.GetToDatabaseAsync(context, new History(DateTime.Now, "Расписание переименовано с "
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