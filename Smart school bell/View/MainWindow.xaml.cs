using System.Windows;
using Smart_school_bell.ViewModel;

namespace Smart_school_bell
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = (MainWindowViewModel) DataContext;
            viewModel.ThisWindow = this;
            DataContext = viewModel;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
