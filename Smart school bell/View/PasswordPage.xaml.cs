using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Smart_school_bell.Model;

namespace Smart_school_bell.View
{
    /// <summary>
    /// Логика взаимодействия для PasswordPage.xaml
    /// </summary>
    public partial class PasswordPage : Page, IDataErrorInfo
    {
        private bool isEmptyPassword = true;
        private bool isEmptyNewPassword = true;
        private bool isEmptyRepearNewPassord = true;

        public PasswordPage()
        {
            InitializeComponent();
            if (PasswordData.ChekEmptyPasswors())
            {
                Password.Visibility = Visibility.Collapsed;
                isEmptyPassword = false;
                EditButton.Content = "Создать";
            }
            CheckFields();
            
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NewPassword.Password == RepeatNewPassword.Password)
            {
                using (var context = new DatabaseContext())
                {
                    if (PasswordData.GetPasswordData().Password == PasswordData.GetHashString(Password.Password))
                    {
                        if (!PasswordData.ChekEmptyPasswors())
                        {
                            context.Passwords.Remove(context.Passwords.Find(PasswordData.GetPasswordData().Id));
                        }

                        context.Passwords.Add(new PasswordData(PasswordData.GetHashString(NewPassword.Password)));
                        context.SaveChanges();

                        Password.Password = "";
                        NewPassword.Password = "";
                        RepeatNewPassword.Password = "";
                        CheckFields();

                        MessageBox.Show("Пароль был изменён", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Неверные пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверно повтарён пароль", "Ошибка создания пароля", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void CheckFields()
        {
            EditButton.IsEnabled = !(isEmptyPassword || isEmptyNewPassword || isEmptyRepearNewPassord);
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                return result;
            }
        }

        public string Error { get; }

        private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            isEmptyPassword = Password.Password == "";
            CheckFields();
        }

        private void RepeatNewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            isEmptyRepearNewPassord = RepeatNewPassword.Password == "";
            CheckFields();
        }

        private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            isEmptyNewPassword = NewPassword.Password == "";
            CheckFields();
        }
    }
}
