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
using Smart_school_bell.ViewModel;

namespace Smart_school_bell.View
{
    /// <summary>
    /// Логика взаимодействия для DialogNewTimetable.xaml
    /// </summary>
    public partial class DialogNewTimetable : UserControl
    {
        public DialogNewTimetable(DialogNewTimetableViewModel dialogViewModel)
        {
            InitializeComponent();
            DataContext = dialogViewModel;
        }
    }
}
