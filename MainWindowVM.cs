using RestaurantIncercareaDoua.Business_Logic_Layer;
using RestaurantIncercareaDoua.Model;
using RestaurantIncercareaDoua.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestaurantIncercareaDoua.ViewModel
{
    internal class MainWindowVM : INotifyPropertyChanged
    {
        private UserBLL userBLL = new UserBLL();
        private string userEmail;
        public string UserEmail
        {
            get { return userEmail; }
            set
            {
                userEmail = value;
                OnPropertyChanged(nameof(UserEmail));
            }
        }

        private string userPassword;
        public string UserPassword
        {
            get { return userPassword; }
            set
            {
                userPassword = value;
                OnPropertyChanged(nameof(UserPassword));
            }
        }
        public ICommand LogInCommand { get; set; }
        public ICommand SignInCommand { get; set; }
        public ICommand GuestCommand { get; set; }

        public MainWindowVM()
        {
            LogInCommand = new RelayCommand(LogIn, CanLogIn);
            SignInCommand = new RelayCommand(SignIn, CanSignIn);
            GuestCommand = new RelayCommand(Guest, CanGuest);
        }

        private bool CanGuest(object obj)
        {
            return true;
        }

        private bool CanSignIn(object obj)
        {
            return true;
        }

        private bool CanLogIn(object obj)
        {
            if(string.IsNullOrEmpty(UserEmail))
            {
                return false;
            }
            return true;
        }

        private void LogIn(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox.Password;
            string? userRole = userBLL.LogIn(UserEmail, password);

            if (userRole == "")
            {
                userRole = null;
            }

            if (userRole != null)
            {
                MessageBox.Show($"Welcome! Role: {userRole}");

                int userId = userBLL.GetUserId(UserEmail);

                UserType rolEnum = UserType.Client;
                if (userRole.ToLower() == "angajat")
                {
                    rolEnum = UserType.Angajat;
                }

                RestaurantMenuWindow menuWindow = new RestaurantMenuWindow(userId, rolEnum);
                menuWindow.Show();

                Application.Current.MainWindow.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password");
            }
        }

        private void SignIn(object parameter)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();
        }

        private void Guest(object parameter)
        {
            RestaurantMenuWindow restaurantMenuWindow = new RestaurantMenuWindow(-1, UserType.Guest);
            restaurantMenuWindow.Show();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
