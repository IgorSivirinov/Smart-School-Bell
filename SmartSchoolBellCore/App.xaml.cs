using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SmartSchoolBellCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow _appWindow;

        public static void ShowMainWindow()
        {
           _appWindow = new();
           _appWindow.Show();
        }

        public static void RestartMainWindow()
        {
            _appWindow.RestartWindow();
        }

    }
}
