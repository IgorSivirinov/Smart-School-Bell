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
using System.Windows.Shapes;
using SmartSchoolBellCore.Model;
using static SmartSchoolBellCore.Services.StartBellService;

namespace SmartSchoolBellCore.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            StartTimerBell();
            if (!PasswordData.CheckEmptyPasswords()) return;
            App.ShowMainWindow();
            Close();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void EnterPas(object sender, MouseEventArgs e)
        {
            if (Password.Password != "") Login();

        }

        private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ButtonLogin.IsEnabled = Password.Password != "";
        }

        private async Task Login()
        {
            if ((await PasswordData.GetPasswordData()).Password == PasswordData.GetHashString(Password.Password))
            {
                App.ShowMainWindow();
                Close();
            }
            else
            {
                MessageBox.Show("Неверные пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void KeyDownPas(object sender, KeyEventArgs e)
        {
            if (Password.Password != "" && e.Key == Key.Enter) Login();
        }

        private async void PasswordReset(object sender, RoutedEventArgs e){
        
             MessageBoxResult result =
                    MessageBox.Show("Опирация удалит ВСЕ данные приложения.\n" +
                                    "Вы хотите продолжить?"
                        , "Сброс пароля", MessageBoxButton.YesNo, MessageBoxImage.Warning);

             if (MessageBoxResult.Yes == result)
             {
                 await using var context = new DatabaseContext();

                 await context.Database.EnsureDeletedAsync();
                 await context.Database.EnsureCreatedAsync();

                App.ShowMainWindow();
                Close();
             }
        }
    }
}
