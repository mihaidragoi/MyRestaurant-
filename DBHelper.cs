using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Data_Access_Layer
{
    internal class DBHelper
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["RestaurantDBMAP"].ConnectionString;

        public Npgsql.NpgsqlConnection GetConnection()
        {
            return new Npgsql.NpgsqlConnection(connectionString);
        }
    }
}
