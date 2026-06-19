using RestaurantIncercareaDoua.Data_Access_Layer;
using RestaurantIncercareaDoua.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.ViewModel
{
    public class OrderDetailsWindowVM
    {
        public OrderDetailsModel Detalii { get; set; }

        public OrderDetailsWindowVM(int idComanda)
        {
            var orderDAL = new OrderDataAccess();
            Detalii = orderDAL.GetDetaliiCompleteComanda(idComanda);
        }
    }
}
