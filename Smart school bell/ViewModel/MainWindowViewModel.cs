using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Smart_school_bell.Annotations;
using Smart_school_bell.Model;
using Smart_school_bell.View;
using System.Linq;

namespace Smart_school_bell.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<MainWindowNavigationItemViewModel> NavigationItemsItemsControl { get; set; }
            = new ObservableCollection<MainWindowNavigationItemViewModel>();

        public ICommand ComGoHistory
        {
            get { return  new DelegateCommand(o =>
            {
                HistoryPage page = new HistoryPage();
                MainFrame = page;
                OnPropertyChanged("MainFrame");
            });}
        }

        public ICommand ComGoSchedules
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    SchedulesPage page = new SchedulesPage();
                    MainFrame = page;
                    OnPropertyChanged("MainFrame");
                });
            }
        }

        public Page MainFrame
        {
            get;
            set;
        }


        public MainWindowViewModel()
        {
            using (var context = new DatabaseContext())
            {
                context.Histories.Add(new History(DateTime.Now, "Вход в приложение"));
            }
            SchedulesPage page = new SchedulesPage();
            MainFrame = page;
            OnPropertyChanged("MainFrame");
        }
    }
}