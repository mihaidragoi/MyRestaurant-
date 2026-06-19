using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Model
{
    public class Preparat
    {
        public int Id { get; set; }
        public int IdCategorie { get; set; }
        public string Denumire { get; set; }
        public decimal Pret { get; set; }
        public decimal CantitatePortie { get; set; }
        public decimal CantitateTotala { get; set; }
        public string ImaginePath { get; set; }


        public string NumeCategorie { get; set; }
        public bool IsMeniu { get; set; } = false;
        public string CantitatiDetaliateMeniu { get; set; } = ""; 
        public bool IsDisponibil { get; set; } = true;

        public List<string> Alergeni { get; set; } = new List<string>();

        public string ListaAlergeniText
        {
            get
            {
                if (!IsDisponibil) return "❌ INDISPONIBIL!";
                return (Alergeni != null && Alergeni.Count > 0) ? string.Join(", ", Alergeni) : "Fără alergeni";
            }
        }
    }
}
