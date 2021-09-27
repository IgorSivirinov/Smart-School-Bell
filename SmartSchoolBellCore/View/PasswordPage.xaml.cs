using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SmartSchoolBellCore.Model;

namespace SmartSchoolBellCore.View
{
    public partial class PasswordPage : Page, IDataErrorInfo
    {
        private bool _isEmptyPassword = true;
        private bool _isEmptyNewPassword = true;
        private bool _isEmptyRepeatNewPassword = true;

        public PasswordPage()
        {
            InitializeComponent();
            if (PasswordData.CheckEmptyPasswords())
            {
                Password.Visibility = Visibility.Collapsed;
                _isEmptyPassword = false;
                EditButton.Content = "Создать";
                DeletePassword.Visibility = Visibility.Collapsed;
                EditButton.Click += (s, e) =>
                {
                    if (NewPassword.Password == RepeatNewPassword.Password)
                    {
                        using var context = new DatabaseContext();
                        context.Passwords.Add(new(PasswordData.GetHashString(NewPassword.Password)));
                        context.SaveChanges();
                        App.RestartMainWindow();
                    }
                    else
                    {
                        MessageBox.Show("Неверно повтарён пароль", "Ошибка создания пароля", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                };
            }
            else
            {
                DeletePassword.Click += (s, e) =>
                {
                    using var context = new DatabaseContext();
                    var passwordData = PasswordData.GetPasswordData().Result;
                    if (passwordData.Password == PasswordData.GetHashString(Password.Password))
                    {
                        if (!PasswordData.CheckEmptyPasswords())
                        {
                            context.Passwords.Remove(passwordData);
                        }

                        context.SaveChanges();

                        MessageBox.Show("Пароль был удалён", "Удаление  пароля", MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        App.RestartMainWindow();
                    }
                    else
                    {
                        MessageBox.Show("Неверные пароль", "Ошибка входа", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                };
                EditButton.Click += (s, e) =>
                {
                    if (NewPassword.Password == RepeatNewPassword.Password)
                    {
                        using var context = new DatabaseContext();
                        var passwordData = PasswordData.GetPasswordData().Result;
                        if (passwordData.Password == PasswordData.GetHashString(Password.Password))
                        {
                            if (!PasswordData.CheckEmptyPasswords())
                            {
                                context.Passwords.Remove(passwordData);
                            }

                            context.Passwords.Add(new PasswordData(PasswordData.GetHashString(NewPassword.Password)));
                            context.SaveChanges();

                            Password.Password = "";
                            NewPassword.Password = "";
                            RepeatNewPassword.Password = "";
                            CheckFields();

                            MessageBox.Show("Пароль был изменён", "Изменение пароля", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Неверные пароль", "Ошибка входа", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверно повтарён пароль", "Ошибка создания пароля", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                };
            }
            CheckFields();
        }


        private void CheckFields()
        {
            EditButton.IsEnabled = !(_isEmptyPassword || _isEmptyNewPassword || _isEmptyRepeatNewPassword);
            DeletePassword.IsEnabled = !_isEmptyPassword;
        }

        public string this[string columnName] => null;

        public string Error { get; }

        private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _isEmptyPassword = Password.Password == "";
            CheckFields();
        }

        private void RepeatNewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _isEmptyRepeatNewPassword = RepeatNewPassword.Password == "";
            CheckFields();
        }

        private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _isEmptyNewPassword = NewPassword.Password == "";
            CheckFields();
        }
    }
}
