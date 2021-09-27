using System.Windows;
using System.Windows.Controls;
using SmartSchoolBellCore.ViewModel;

namespace SmartSchoolBellCore.View
{
    public partial class DialogRenameTimetable : UserControl
    {
        public DialogRenameTimetable(DialogRenameTimetableViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
