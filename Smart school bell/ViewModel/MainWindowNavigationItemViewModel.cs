using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Smart_school_bell.Annotations;
using Smart_school_bell.Model;

namespace Smart_school_bell.ViewModel
{
    public class MainWindowNavigationItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static event Action PropertySelectedEventHandler;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static int _idSelected;
        public bool IsSelected
        {
            get
            {
                return Timetable.Id == _idSelected;
            }
        }
        
       
        public ICommand MouseEnter { get; set; }
        public ICommand MouseLeave { get; set; }
        
        public Timetable Timetable { get; set; }
        public string ItemBackground { get; set; }

        public ICommand ComDelete { get; set; }

        public ICommand ComClick { get; set; }
        public event Action EventClickTimetable; 

        public event Action EventRenameTimetable;
        public ICommand ComRenameTimetable { get; set; }

        public event Action EventDeleteItem;

        public MainWindowNavigationItemViewModel(Timetable timetable)
        {
            Timetable = timetable;
            ComClick = new DelegateCommand(o=>
            {
                _idSelected = Timetable.Id;
                PropertySelectedEventHandler();
                EventClickTimetable();
                OnPropertyChanged("IsSelected");
            });
            MouseEnter = new DelegateCommand(o =>
            {
                ItemBackground = "#F5F5F5";
                OnPropertyChanged("ItemBackground");
            });
            MouseLeave = new DelegateCommand(o =>
            {
                ItemBackground = "#FFF";
                OnPropertyChanged("ItemBackground");
            });
            PropertySelectedEventHandler += OnPropertySelectedEventHandler;
            ComDelete = new DelegateCommand(o=> EventDeleteItem());
            ComRenameTimetable = new DelegateCommand(o =>
            {
                EventRenameTimetable();
            });
        }

        protected virtual void OnPropertySelectedEventHandler()
        {
            OnPropertyChanged("IsSelected");
        }
    }
}