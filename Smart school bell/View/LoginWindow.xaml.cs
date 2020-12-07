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
using Smart_school_bell.Model;

namespace Smart_school_bell.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Timetable.StartTimerBell();
            if (PasswordData.ChekEmptyPasswors())
            {
                MainWindow window = new MainWindow();
                window.Show();
                Close();
            }
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
            if (Password.Password == "") ButtonLogin.IsEnabled = false;
            else ButtonLogin.IsEnabled = true;
        }

        private void Login()
        {
            using (var context = new DatabaseContext())
            {
                if (PasswordData.GetPasswordData().Password==PasswordData.GetHashString(Password.Password))
                {
                    MainWindow window = new MainWindow();
                    window.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверные пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void KeyDownPas(object sender, KeyEventArgs e)
        {
            if (Password.Password != "" && e.Key == Key.Enter) Login();
        }

        private void PasswordReset(object sender, RoutedEventArgs e){
        
             MessageBoxResult result =
                    MessageBox.Show("Опирация удалит ВСЕ данные приложения.\n" +
                                    "Вы хотите продолжить?"
                        , "Сброс пароля", MessageBoxButton.YesNo, MessageBoxImage.Warning);
             if (MessageBoxResult.Yes == result)
             {
                 using (var context = new DatabaseContext())
                 {

                     foreach (var data in context.Histories)
                     {
                         using (var context2 = new DatabaseContext())
                         {
                             context2.Histories.Remove(context2.Histories.Find(data.Id));
                             context2.SaveChanges();
                         }
                     }

                     foreach (var data in context.Timetables)
                     {
                         using (var context2 = new DatabaseContext())
                         {
                             Timetable timetable = context2.Timetables.Find(data.Id);

                             context2.TimetableDayOfWeeks.Remove(timetable.Monday);
                             context2.TimetableDayOfWeeks.Remove(timetable.Tuesday);
                             context2.TimetableDayOfWeeks.Remove(timetable.Wednesday);
                             context2.TimetableDayOfWeeks.Remove(timetable.Thursday);
                             context2.TimetableDayOfWeeks.Remove(timetable.Friday);
                             context2.TimetableDayOfWeeks.Remove(timetable.Saturday);
                             context2.SaveChanges();

                             context2.Timetables.Remove(context2.Timetables.Find(data.Id));
                             context2.SaveChanges();
                        }
                     }

                     foreach (var data in context.Passwords)
                     {
                         using (var context2 = new DatabaseContext())
                         {
                             context2.Passwords.Remove(context2.Passwords.Find(data.Id));
                             context2.SaveChanges();
                         }

                     }

                     
                 }
                 MainWindow window = new MainWindow();
                 window.Show();
                 Close();
             }
        }
    }
}
