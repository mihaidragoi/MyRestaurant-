using RestaurantIncercareaDoua.Business_Logic_Layer;
using RestaurantIncercareaDoua.Data_Access_Layer;
using RestaurantIncercareaDoua.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantIncercareaDoua.ViewModel
{
    public class CartWindowVM : INotifyPropertyChanged
    {
        private OrderBLL orderBLL = new OrderBLL();

        public ObservableCollection<CartItem> CartItems { get; set; }
        public decimal TotalPlata => CartItems.Sum(item => item.Subtotal);

        public int CurentUserId { get; set; } = -1;

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

        public ICommand PlaceOrderCommand { get; set; }
        public ICommand ResetCartCommand { get; set; }

        public CartWindowVM(ObservableCollection<CartItem> cartItems)
        {
            CartItems = cartItems;

            CartItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPlata));
            foreach (var item in CartItems)
            {
                item.PropertyChanged += (s, e) => OnPropertyChanged(nameof(TotalPlata));
            }

            PlaceOrderCommand = new RelayCommand(PlaceOrderImpl, CanPlaceOrder);
            ResetCartCommand = new RelayCommand(ResetCartImpl, CanResetCart);
        }

        private bool CanPlaceOrder(object parameter)
        {
            return CartItems.Count > 0 && !string.IsNullOrEmpty(UserAddress) && !string.IsNullOrEmpty(UserPhone);
        }

        private void PlaceOrderImpl(object parameter)
        {
            string rezultat = orderBLL.PlaceOrder(this.CurentUserId, UserAddress, UserPhone, CartItems);

            if (rezultat == "Succes")
            {
                MessageBox.Show("Comandă plasată cu succes! Poți urmări statusul ei din meniu.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                CartItems.Clear();

                if (parameter is Window window)
                {
                    window.Close();
                }
            }
            else
            {
                MessageBox.Show(rezultat, "Eroare la plasarea comenzii", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanResetCart(object parameter) => CartItems.Count > 0;

        private void ResetCartImpl(object parameter)
        {
            CartItems.Clear();
        }

        public void ConfigurareInitialaDateLivrare(string adresaDinProfil, string telefonDinProfil, int idUtilizator)
        {
            this.UserAddress = adresaDinProfil;
            this.UserPhone = telefonDinProfil;
            this.CurentUserId = idUtilizator;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}