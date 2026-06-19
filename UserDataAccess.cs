using Npgsql;
using RestaurantIncercareaDoua.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Data_Access_Layer
{
    public class UserDataAccess
    {
        private DBHelper dbHelper = new DBHelper();
        public void RegisterUser(string firstName, string lastName, string email, string phone, string address, string password, string role)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();

                string sql = "CALL insert_utilizator(@n, @p, @e, @t, @a, @pass, @r)";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("n", lastName ?? "");
                    command.Parameters.AddWithValue("p", firstName ?? "");
                    command.Parameters.AddWithValue("e", email);
                    command.Parameters.AddWithValue("t", phone ?? "");
                    command.Parameters.AddWithValue("a", address ?? "");
                    command.Parameters.AddWithValue("pass", password);
                    command.Parameters.AddWithValue("r", role);

                    command.ExecuteNonQuery();
                }
            }
        }

        public string? VerifyUser(string email, string password)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                string sql = "CALL verify_utilizator(@e, @pass, @rol)";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("e", email);
                    command.Parameters.AddWithValue("pass", password);
                    var roleParam = new NpgsqlParameter("rol", NpgsqlTypes.NpgsqlDbType.Text);
                    roleParam.Direction = System.Data.ParameterDirection.InputOutput;
                    roleParam.Value = DBNull.Value; 
                    command.Parameters.Add(roleParam);

                    command.ExecuteNonQuery();

                    return roleParam.Value?.ToString();
                }
            }
        }

        public string UpdateDateUtilizator(int idUser, string telefon, string adresa, string parola)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string sql = "CALL update_date_utilizator(@id, @tel, @adr, @par)";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("id", idUser);
                        cmd.Parameters.AddWithValue("tel", telefon ?? "");
                        cmd.Parameters.AddWithValue("adr", adresa ?? "");
                        cmd.Parameters.AddWithValue("par", parola ?? "");

                        int randuriModificate = cmd.ExecuteNonQuery();

                        if (randuriModificate == 0)
                        {
                            return $"Nu s-a găsit utilizatorul cu ID-ul {idUser} în baza de date.";
                        }

                        return "Succes";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Eroare SQL: {ex.Message}";
            }
        }

        public int GetUserIdByEmail(string email)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                string sql = "SELECT id_user FROM \"Utilizatori\" WHERE email = @email";
                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("email", email);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            return -1;
        }

        public User GetUserById(int userId)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                string sql = "SELECT * FROM \"Utilizatori\" WHERE id_user = @id";
                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                idUser = reader.GetInt32(0),
                                userEmail = reader.GetString(reader.GetOrdinal("email")),
                                userPhone = reader.GetString(reader.GetOrdinal("telefon")),
                                userAddress = reader.GetString(reader.GetOrdinal("adresaLivrare")),
                                userPassword = reader.GetString(reader.GetOrdinal("parola"))
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}
