using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantIncercareaDoua.Model
{
    public class Order : INotifyPropertyChanged
    {
        public int IdComanda { get; set; }
        public DateTime DataComanda { get; set; }
        public DateTime? OraEstimativa { get; set; }
        public decimal SumaTotala { get; set; }
        private string status;
        public string Status
        {
            get { return status; }
            set 
            { 
                status = value; 
                OnPropertyChanged(nameof(Status)); 
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
