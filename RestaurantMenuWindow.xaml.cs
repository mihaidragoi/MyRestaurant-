using RestaurantIncercareaDoua.Model;
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

namespace RestaurantIncercareaDoua.View
{
    /// <summary>
    /// Interaction logic for RestaurantMenuWindow.xaml
    /// </summary>
    public partial class RestaurantMenuWindow : Window
    {
        public RestaurantMenuWindow(int idLogat = -1, UserType rol = UserType.Guest)
        {
            InitializeComponent();
            DataContext = new ViewModel.RestaurantMenuWindowVM(idLogat, rol);
        }
    }
}
