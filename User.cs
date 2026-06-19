using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Model
{
    public class User
    {
        public int idUser { get; set; }
        public string? userFirstName { get; set; }
        public string? userLastName { get; set; }
        public string? userEmail { get; set; }
        public string? userPhone { get; set; }
        public string? userAddress { get; set; }
        public string? userPassword { get; set; }
        public string? userRole { get; set; }

    }
}
