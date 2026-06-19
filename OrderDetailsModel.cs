using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Model
{
    public class OrderDetailsModel
    {
        public string NumeClient { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }

        public int IdComanda { get; set; }
        public DateTime DataComanda { get; set; }
        public DateTime? OraEstimativa { get; set; } 
        public string Status { get; set; }
        public decimal CostTotal { get; set; }

        public List<PreparatComandat> Preparate { get; set; } = new List<PreparatComandat>();

        public decimal CostMancare => Preparate.Sum(p => p.Subtotal);

        public decimal CostTransport => CostTotal - CostMancare;
    }
}
