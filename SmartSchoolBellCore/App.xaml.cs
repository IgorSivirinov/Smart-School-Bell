using SmartSchoolBellCore.Model;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SmartSchoolBellCore
{
    public partial class App : Application
    {
        public static IDisposable BellObservableDisposable { get; set; }
        public static Dispatcher StaticDispatcher = Dispatcher.CurrentDispatcher;

        public static MediaPlayer Player = new();
        private static MainWindow _appWindow;

        public App()
        {
            Player.MediaFailed += (s, e) =>
            {
                MessageBox.Show("Проверьте расположение файла",
                    "Ошибка воспроизведения файла (TtSTB1)",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            };
            Player.MediaOpened += (s, e) =>
            {
                Player.Play();
            };
        }

        public static void ShowMainWindow()
        {
            Task.Run(async () => await History.GetToDatabaseAsync(new(), new History(DateTime.Now, "Вход в приложение")));
            _appWindow = new();
           _appWindow.Show();
        }

        public static void RestartMainWindow()
        {
            _appWindow.RestartWindow();
        }

    }
}
