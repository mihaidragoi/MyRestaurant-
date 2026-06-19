using RestaurantIncercareaDoua.Business_Logic_Layer;
using RestaurantIncercareaDoua.Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantIncercareaDoua.ViewModel
{
    internal class SignInWindowVM : INotifyPropertyChanged
    {
        private UserBLL userBLL = new UserBLL();
        private string userFirstName;
        public string UserFirstName
        {
            get { return userFirstName; }
            set
            {
                userFirstName = value;
                OnPropertyChanged(nameof(UserFirstName));
            }
        }
        private string userLastName;
        public string UserLastName
        {
            get { return userLastName; }
            set
            {
                userLastName = value;
                OnPropertyChanged(nameof(UserLastName));
            }
        }
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
        private string userPhone;
        public string UserPhone
        {
            get { return userPhone; }
            set
            {
                userPhone = value;
                OnPropertyChanged(nameof(UserPhone));
            }
        }
        private string userAddress;
        public string UserAddress
        {
            get { return userAddress; }
            set
            {
                userAddress = value;
                OnPropertyChanged(nameof(UserAddress));
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

        public ICommand SaveCommand { get; set; }

        public SignInWindowVM()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
        }

        private bool CanSave(object obj)
        {
            if (string.IsNullOrEmpty(UserFirstName) || string.IsNullOrEmpty(UserLastName) || string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPhone) || string.IsNullOrEmpty(UserAddress) || string.IsNullOrEmpty(UserPassword))
            {
                return false;
            }
            return true;
        }

        private void Save(object obj)
        {
            string result = userBLL.CreateAccount(UserFirstName, UserLastName, UserEmail, UserPhone, UserAddress, UserPassword);

            if (result == "Account created successfully.")
            {
                MessageBox.Show(result);
            }
            else
            {
                MessageBox.Show("Atenție: " + result);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
