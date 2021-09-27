using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartSchoolBellCore.ViewModel;

namespace SmartSchoolBellCore.View
{
    /// <summary>
    /// Логика взаимодействия для DialogNewTimeBell.xaml
    /// </summary>
    public partial class DialogNewTimeBell : UserControl
    {
        public DialogNewTimeBell(DialogNewTimeBellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
