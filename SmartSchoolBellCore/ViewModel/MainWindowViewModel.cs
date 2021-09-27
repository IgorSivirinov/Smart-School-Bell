using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SmartSchoolBellCore.Model;
using SmartSchoolBellCore.View;

namespace SmartSchoolBellCore.ViewModel
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        

        private Page _mainPage;
        public Page MainFrame
        {
            get => _mainPage;
            set
            {
                _mainPage = value;
                OnPropertyChanged(nameof(MainFrame));
            }
        }

        public Window ThisWindow { get; set; }

        public Visibility VisibilityButtonOut { get; set; } = Visibility.Hidden;

        public ObservableCollection<MainWindowNavigationItemViewModel> NavigationItemsItemsControl { get; set; } = new();

        public ICommand ComGoHistory => new DelegateCommand(o => MainFrame = new HistoryPage());
        public ICommand ComGoSchedules => new DelegateCommand(o => MainFrame = new SchedulesPage());
        public ICommand ComGoPassword => new DelegateCommand(o => MainFrame = new PasswordPage());
        public ICommand ComLogOff => new DelegateCommand(o =>
            {
                var window = new LoginWindow();
                window.Show();
                ThisWindow.Close();
            });

        public MainWindowViewModel()
        {
            History.GetToDatabaseAsync(new (), new History(DateTime.Now, "Вход в приложение"));

            MainFrame = new SchedulesPage();

            if (!PasswordData.CheckEmptyPasswords())
            {
                VisibilityButtonOut = Visibility.Visible;
                OnPropertyChanged(nameof(VisibilityButtonOut));
            }
        }
    }
}