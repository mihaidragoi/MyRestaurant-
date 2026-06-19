using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Model
{
    public class PreparatComandat
    {
        public string Denumire { get; set; }
        public int Cantitate { get; set; }
        public decimal PretBucata { get; set; }
        public decimal Subtotal => Cantitate * PretBucata;
    }
}
