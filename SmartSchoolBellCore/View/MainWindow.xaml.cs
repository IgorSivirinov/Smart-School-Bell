using System.Windows;
using SmartSchoolBellCore.ViewModel;

namespace SmartSchoolBellCore
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            RestartWindow();
        }

        public void RestartWindow()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ThisWindow = this;
            DataContext = viewModel;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
