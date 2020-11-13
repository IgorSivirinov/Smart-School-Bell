using System.Windows.Controls;
using Smart_school_bell.ViewModel;

namespace Smart_school_bell.View
{
    /// <summary>
    /// Логика взаимодействия для TimetablePage.xaml
    /// </summary>
    public partial class TimetablePage : Page
    {
        public TimetablePage(TimetablePageVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
