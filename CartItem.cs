using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantIncercareaDoua.Model
{
    public class CartItem : INotifyPropertyChanged
    {
        public Preparat product { get; set; }
        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value > 0 && value <= product.CantitateTotala)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Subtotal));
                }
                else if (value > product.CantitateTotala)
                {
                    MessageBox.Show($"Stoc insuficient! Avem doar {product.CantitateTotala} porții disponibile.");
                    OnPropertyChanged(nameof(Quantity)); 
                }
                else
                {
                    MessageBox.Show("Cantitatea trebuie să fie cel puțin 1!");
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        private decimal subtotal;
        public decimal Subtotal
        {
            get { return product.Pret * quantity; }
        }

        public CartItem(Preparat preparat, int cantitate)
        {
            product = preparat;
            Quantity = cantitate;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
