using Npgsql;
using RestaurantIncercareaDoua.Model;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace RestaurantIncercareaDoua.Data_Access_Layer
{
    public class MenuDataAccess
    {
        private DBHelper dbHelper = new DBHelper();

        public List<Preparat> GetAllPreparate()
        {
            List<Preparat> preparate = new List<Preparat>();
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                string sql = "SELECT * FROM get_toate_preparatele_cu_alergeni()";

                using (var command = new NpgsqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var prep = new Preparat
                        {
                            Id = reader.GetInt32(0),
                            IdCategorie = reader.GetInt32(1),
                            Denumire = reader.GetString(2),
                            Pret = reader.GetDecimal(3),
                            CantitatePortie = reader.GetDecimal(4),
                            CantitateTotala = reader.GetDecimal(5),
                            NumeCategorie = reader.GetString(6),
                            ImaginePath = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            IsMeniu = false,
                            IsDisponibil = reader.GetDecimal(5) > 0,
                            Alergeni = new List<string>()
                        };

                        if (!reader.IsDBNull(8))
                        {
                            prep.Alergeni = reader.GetString(8).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }

                        preparate.Add(prep);
                    }
                }
            }
            return preparate;
        }

        public List<Preparat> GetAllMeniuri()
        {
            List<Preparat> meniuri = new List<Preparat>();
            decimal discountProcent = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["MenuDiscountPercentage"] ?? "0");

            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                string sql = "SELECT * FROM get_toate_meniurile_cu_pret_calculat()";

                using (var cmd = new NpgsqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal pretIntreg = reader.GetDecimal(4);
                        decimal pretFinal = pretIntreg - (pretIntreg * (discountProcent / 100m));
                        decimal stocMinim = reader.GetDecimal(5);

                        var meniu = new Preparat
                        {
                            Id = reader.GetInt32(0),
                            IdCategorie = reader.GetInt32(1),
                            Denumire = reader.GetString(2),
                            NumeCategorie = reader.GetString(3),
                            Pret = Math.Round(pretFinal, 2),
                            CantitatiDetaliateMeniu = $"{reader.GetString(6)} - {reader.GetString(7)}",
                            IsMeniu = true,
                            CantitateTotala = stocMinim, 
                            IsDisponibil = stocMinim > 0,
                            ImaginePath = "",
                            Alergeni = new List<string>()
                        };

                        if (!reader.IsDBNull(8))
                        {
                            meniu.Alergeni = reader.GetString(8).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }

                        meniuri.Add(meniu);
                    }
                }
            }
            return meniuri;
        }

        public List<string> GetPreparateEpuizate(int limita)
        {
            List<string> epuizate = new List<string>();
            using (var connection = dbHelper.GetConnection())
            {
                connection.Open();
                string sql = "SELECT denumire, cantitate_totala FROM \"Preparate\" WHERE cantitate_totala <= @limita";
                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("limita", limita);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            epuizate.Add($"{reader.GetString(0)} - {reader.GetDecimal(1)}");
                        }
                    }
                }
            }
            return epuizate;
        }

        public string UpdatePreparat(int idPreparat, string denumire, decimal pret, decimal cantitatePortie, decimal cantitateTotala)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string sql = "CALL update_preparat(@id, @den, @pret, @cantP, @cantT)";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("id", idPreparat);
                        cmd.Parameters.AddWithValue("den", denumire);
                        cmd.Parameters.AddWithValue("pret", pret);
                        cmd.Parameters.AddWithValue("cantP", cantitatePortie);
                        cmd.Parameters.AddWithValue("cantT", cantitateTotala);
                        cmd.ExecuteNonQuery();
                    }
                }
                return "Succes";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string AdaugaPreparat(int idCategorie, string denumire, decimal pret, decimal cantitatePortie, decimal cantitateTotala, string imaginePath)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string sql = "CALL insert_preparat_nou(@idCat, @den, @pret, @cantP, @cantT, @img)";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("idCat", idCategorie);
                        cmd.Parameters.AddWithValue("den", denumire);
                        cmd.Parameters.AddWithValue("pret", pret);
                        cmd.Parameters.AddWithValue("cantP", cantitatePortie);
                        cmd.Parameters.AddWithValue("cantT", cantitateTotala);
                        cmd.Parameters.AddWithValue("img", imaginePath ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
                return "Succes";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string StergePreparat(int idPreparat)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string sql = "CALL sterge_preparat(@id)";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("id", idPreparat);
                        cmd.ExecuteNonQuery();
                    }
                }
                return "Succes";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}