using System.Windows.Controls;
using SmartSchoolBellCore.ViewModel;

namespace SmartSchoolBellCore.View
{
    /// <summary>
    /// Логика взаимодействия для TimetablePage.xaml
    /// </summary>
    public partial class TimetablePage : Page
    {
        public TimetablePage(TimetablePageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
