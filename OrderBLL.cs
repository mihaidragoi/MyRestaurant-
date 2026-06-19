using RestaurantIncercareaDoua.Data_Access_Layer;
using RestaurantIncercareaDoua.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Business_Logic_Layer
{
    public class OrderBLL
    {
        private OrderDataAccess orderDAL = new OrderDataAccess();

        public string PlaceOrder(int idClient, string adresa, string telefon, ObservableCollection<CartItem> produse)
        {
            if (produse == null || produse.Count == 0) return "Coșul este gol!";

            decimal costMancare = produse.Sum(p => p.Subtotal);

            decimal sumaTransport_a = decimal.Parse(ConfigurationManager.AppSettings["FreeShippingThreshold"] ?? "0");
            decimal costTransport_b = decimal.Parse(ConfigurationManager.AppSettings["ShippingCost"] ?? "0");
            decimal sumaDiscount_y = decimal.Parse(ConfigurationManager.AppSettings["DiscountThresholdAmount"] ?? "0");
            int comenzi_z = int.Parse(ConfigurationManager.AppSettings["MinimumOrdersForDiscount"] ?? "0");
            int zile_t = int.Parse(ConfigurationManager.AppSettings["OrderIntervalDays"] ?? "0");
            decimal procent_w = decimal.Parse(ConfigurationManager.AppSettings["OrderDiscountPercentage"] ?? "0");

            decimal transportFinal = costMancare < sumaTransport_a ? costTransport_b : 0;

            decimal valoareDiscount = 0;
            int comenziAnterioare = orderDAL.GetNumarComenziRecente(idClient, zile_t);

            if (costMancare > sumaDiscount_y || comenziAnterioare > comenzi_z)
            {
                valoareDiscount = costMancare * (procent_w / 100m);
            }

            decimal totalPlata = (costMancare + transportFinal) - valoareDiscount;

            return orderDAL.InsertOrder(idClient, adresa, telefon, costMancare, transportFinal, valoareDiscount, totalPlata, produse);
        }
    }
}
