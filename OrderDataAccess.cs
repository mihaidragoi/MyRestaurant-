using Npgsql;
using RestaurantIncercareaDoua.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Data_Access_Layer
{
    public class OrderDataAccess
    {
        private DBHelper dbHelper = new DBHelper();

        public int GetNumarComenziRecente(int idClient, int zile)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM \"Comenzi\" WHERE id_user = @client AND \"dataComanda\" >= CURRENT_DATE - CAST(@zile AS INT)";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("client", idClient);
                    cmd.Parameters.AddWithValue("zile", zile);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public string InsertOrder(int idClient, string adresa, string telefon, decimal costMancare, decimal costTransport, decimal discount, decimal total, ObservableCollection<CartItem> produse)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sqlComanda = "SELECT insert_comanda(@id, @tot)";
                        int idComandaNoua;

                        using (var cmd = new NpgsqlCommand(sqlComanda, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("id", idClient);
                            cmd.Parameters.AddWithValue("tot", total); 

                            idComandaNoua = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        string sqlPreparat = "CALL insert_comanda_preparat(@idCmd, @idPrep, @cant, @isMeniu)";
                        foreach (var item in produse)
                        {
                            using (var cmdPrep = new NpgsqlCommand(sqlPreparat, connection, transaction))
                            {
                                cmdPrep.Parameters.AddWithValue("idCmd", idComandaNoua);
                                cmdPrep.Parameters.AddWithValue("idPrep", item.product.Id);
                                cmdPrep.Parameters.AddWithValue("cant", item.Quantity);
                                cmdPrep.Parameters.AddWithValue("isMeniu", item.product.IsMeniu); 
                                cmdPrep.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return "Succes";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return $"Eroare DAL: {ex.Message}";
                    }
                }
            }
        }

        public List<Order> GetIstoricComenzi(int idClient)
        {
            List<Order> istoric = new List<Order>();
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT id_comanda, \"dataComanda\", \"costTotal\", stare, \"oraEstimativa\" FROM \"Comenzi\" WHERE id_user = @client ORDER BY \"dataComanda\" DESC";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("client", idClient);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            istoric.Add(new Order
                            {
                                IdComanda = reader.GetInt32(0),
                                DataComanda = reader.GetDateTime(1),
                                SumaTotala = reader.GetDecimal(2),
                                Status = reader.GetString(3).Trim(),
                                OraEstimativa = reader.IsDBNull(4) ? null : (DateTime?)reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return istoric;
        }

        public List<Order> GetToateComenzile()
        {
            List<Order> toate = new List<Order>();
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT id_comanda, \"dataComanda\", \"costTotal\", stare, \"oraEstimativa\" FROM \"Comenzi\" ORDER BY \"dataComanda\" DESC";

                using (var cmd = new NpgsqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        toate.Add(new Order
                        {
                            IdComanda = reader.GetInt32(0),
                            DataComanda = reader.GetDateTime(1),
                            SumaTotala = reader.GetDecimal(2),
                            Status = reader.GetString(3).Trim(),
                            OraEstimativa = reader.IsDBNull(4) ? null : (DateTime?)reader.GetDateTime(4)
                        });
                    }
                }
            }
            return toate;
        }

        public void UpdateStatusComanda(int idComanda, string noulStatus)
        {
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "CALL update_status_comanda(@id, @status)";
                        using (var cmd = new NpgsqlCommand(sql, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("id", idComanda);
                            cmd.Parameters.AddWithValue("status", noulStatus);
                            cmd.ExecuteNonQuery();
                        }

                        if (noulStatus.Trim().ToLower() == "livrata")
                        {
                            string sqlScadePrep = @"UPDATE ""Preparate"" p SET cantitate_totala = p.cantitate_totala - dc.cantitate FROM ""DetaliiComanda"" dc WHERE p.id_preparat = dc.id_preparat AND dc.id_comanda = @idCmd";
                            using (var cmdStoc = new NpgsqlCommand(sqlScadePrep, connection, transaction))
                            {
                                cmdStoc.Parameters.AddWithValue("idCmd", idComanda);
                                cmdStoc.ExecuteNonQuery();
                            }

                            string sqlScadeMeniu = @"UPDATE ""Preparate"" p SET cantitate_totala = p.cantitate_totala - dc.cantitate FROM ""DetaliiComanda"" dc JOIN ""MeniuPreparate"" mp ON dc.id_meniu = mp.id_meniu WHERE p.id_preparat = mp.id_preparat AND dc.id_comanda = @idCmd";
                            using (var cmdStocM = new NpgsqlCommand(sqlScadeMeniu, connection, transaction))
                            {
                                cmdStocM.Parameters.AddWithValue("idCmd", idComanda);
                                cmdStocM.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public OrderDetailsModel GetDetaliiCompleteComanda(int idComanda)
        {
            OrderDetailsModel detalii = new OrderDetailsModel();
            decimal discountProcent = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["MenuDiscountPercentage"] ?? "0");

            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();

                string sqlClient = @"SELECT u.nume, u.prenume, u.telefon, u.""adresaLivrare"", c.""costTotal"", c.stare, c.id_comanda, c.""dataComanda"", c.""oraEstimativa"" FROM ""Comenzi"" c JOIN ""Utilizatori"" u ON c.id_user = u.id_user WHERE c.id_comanda = @idCmd";

                using (var cmd = new NpgsqlCommand(sqlClient, connection))
                {
                    cmd.Parameters.AddWithValue("idCmd", idComanda);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            detalii.NumeClient = reader.GetString(0) + " " + reader.GetString(1);
                            detalii.Telefon = reader.GetString(2);
                            detalii.Adresa = reader.GetString(3);
                            detalii.CostTotal = reader.GetDecimal(4);
                            detalii.Status = reader.GetString(5);
                            detalii.IdComanda = reader.GetInt32(6);
                            detalii.DataComanda = reader.GetDateTime(7);

                            if (!reader.IsDBNull(8))
                            {
                                detalii.OraEstimativa = reader.GetDateTime(8);
                            }
                        }
                    }
                }

                string sqlPreparate = @"SELECT p.denumire, dc.cantitate, p.pret FROM ""DetaliiComanda"" dc JOIN ""Preparate"" p ON dc.id_preparat = p.id_preparat WHERE dc.id_comanda = @idCmd";
                using (var cmdP = new NpgsqlCommand(sqlPreparate, connection))
                {
                    cmdP.Parameters.AddWithValue("idCmd", idComanda);
                    using (var reader = cmdP.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            detalii.Preparate.Add(new PreparatComandat
                            {
                                Denumire = reader.GetString(0),
                                Cantitate = reader.GetInt32(1),
                                PretBucata = reader.GetDecimal(2)
                            });
                        }
                    }
                }

                string sqlMeniuri = @"SELECT m.denumire, dc.cantitate, (SELECT SUM(p2.pret) FROM ""MeniuPreparate"" mp JOIN ""Preparate"" p2 ON mp.id_preparat = p2.id_preparat WHERE mp.id_meniu = m.id_meniu) AS pret_brut FROM ""DetaliiComanda"" dc JOIN ""Meniuri"" m ON dc.id_meniu = m.id_meniu WHERE dc.id_comanda = @idCmd";

                using (var cmdM = new NpgsqlCommand(sqlMeniuri, connection))
                {
                    cmdM.Parameters.AddWithValue("idCmd", idComanda);
                    using (var reader = cmdM.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal pretBrut = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                            decimal pretFinalMeniu = pretBrut - (pretBrut * (discountProcent / 100m));

                            detalii.Preparate.Add(new PreparatComandat
                            {
                                Denumire = reader.GetString(0) + " (Meniu)",
                                Cantitate = reader.GetInt32(1),
                                PretBucata = Math.Round(pretFinalMeniu, 2)
                            });
                        }
                    }
                }
            }

            return detalii;
        }

    }
}