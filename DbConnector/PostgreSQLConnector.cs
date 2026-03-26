using Npgsql;
using System;
using System.Configuration;
using System.Data;

namespace WebAppAMIU.DbConnector
{
    public class PostgreSQLConnector
    {

        private NpgsqlConnection conn;

        public static void AssignStringValue(NpgsqlParameter p, string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                p.Value = DBNull.Value;
            }
            else
            {
                p.Value = val;
            }
        }

        public static void AssignDateValue(NpgsqlParameter p, string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                p.Value = DBNull.Value;
            }
            else
            {
                p.Value = val;
            }
        }


        // conversione di una data in formato utile da inserire in una istruzione PostGres
        public static string ManageDtOrNullValForPS(DateTime? v)
        {
            if (v == null) { return "null"; }
            ;

            DateTime d = (DateTime)v.Value;

            return "'" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + "'";
        }

        public static string ManageDtOrNullValForPS(DataRow dr, string colonna)
        {
            if (dr[colonna] == DBNull.Value) { return "null"; }
            ;

            return "'" + Convert.ToDateTime(dr[colonna]).ToString("yyyy-MM-dd") + "'";
        }


        public static string ManageDecOrNullVal(string v)
        {
            if (string.IsNullOrEmpty(v)) { return "null"; }
            ;

            return v.Replace(",", ".");
        }



        // conversione di una intero in formato utile da inserire in una istruzione PostGres
        public static string ManageIntOrNullValForPS(int? v)
        {
            if (v == null) { return "null"; }
            ;

            return v.ToString();
        }

        public static string ManageIntOrNullValForPS(DataRow dr, string colonna)
        {
            if (dr[colonna] == DBNull.Value) { return "null"; }
            ;

            return dr[colonna].ToString();
        }

        // conversione di una stringa in formato utile da inserire in una istruzione PostGres
        public static string ManageStringOrNullVal(string v)
        {
            if (string.IsNullOrWhiteSpace(v?.Trim())) { return "null"; }
            ;

            v = v.Replace("'", "''");

            return "'" + v.Trim() + "'";
        }
        public static string ManageStringOrNullVal(DataRow dr, string colonna)
        {
            if (dr[colonna] == DBNull.Value) { return "null"; }
            ;

            return "'" + dr[colonna].ToString().Replace("'", "''") + "'";
        }

        public static DateTime? ConvDate(DataRow dr, string colName, string format = "yyyy-MM-dd")
        {
            if (string.IsNullOrEmpty(dr[colName].ToString())) { return null; }

            return Convert.ToDateTime(dr[colName]);
        }

        public static string ConvDate2String(DataRow dr, string colName, string format = "yyyy-MM-dd")
        {
            if (string.IsNullOrEmpty(dr[colName].ToString())) { return ""; }

            return Convert.ToDateTime(dr[colName]).ToString(format);
        }

        internal PostgreSQLConnector(string connectionStringKey)
        {
            if (ConfigurationManager.ConnectionStrings[connectionStringKey] != null)
            {
                conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString);
                conn.Open();
            }
        }

        internal NpgsqlCommand CreateCommand(string sqlCommandText, NpgsqlTransaction transaction = null)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommandText;
                return cmd;
            }
            return null;
        }

        internal NpgsqlConnection Connection { get { return conn; } }

        internal void Dispose()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
        }



        // versione "non transazionale"
        public static int ExecuteNonQuery(string connStrSettings, string strSrc, params NpgsqlParameter[] arrParam)
        {
            var dt = new DataTable();
            var connStr = ConfigurationManager.ConnectionStrings[connStrSettings].ConnectionString;

            Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connStr);
            conn.Open();
            Npgsql.NpgsqlCommand cmd = conn.CreateCommand();

            try
            {
                cmd.CommandText = strSrc;
                foreach (var param in arrParam)
                {
                    cmd.Parameters.Add(param);
                }
                var ret = cmd.ExecuteNonQuery();
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        // versione che gestisce transazioni e ritona anche la chiave auto generata - identity
        // la tarnsazione la deve aprire il chiamante
        // nella string di inserimento deve esserci il "returning id"
        public static int ExecuteInsertWithRetId(string strSrc, NpgsqlConnection conn, params NpgsqlParameter[] arrParam)
        {
            var dt = new DataTable();
            Npgsql.NpgsqlCommand cmd = conn.CreateCommand();

            try
            {
                cmd.CommandText = strSrc;
                foreach (var param in arrParam)
                {
                    cmd.Parameters.Add(param);
                }
                var ret = cmd.ExecuteScalar();
                return Convert.ToInt32(ret);
            }
            catch (Exception)
            {
                throw;
            }
        }


        // versione che gestisce transazioni ( prevede sia stata aperta prima ..)
        public static int ExecuteNonQuery(string strSrc, NpgsqlConnection conn, params NpgsqlParameter[] arrParam)
        {
            var dt = new DataTable();
            Npgsql.NpgsqlCommand cmd = conn.CreateCommand();

            try
            {
                cmd.CommandText = strSrc;
                foreach (var param in arrParam)
                {
                    cmd.Parameters.Add(param);
                }
                var ret = cmd.ExecuteNonQuery();
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // versione nn transazionale
        public static DataTable ExecuteReader(string connStrSettings, string strSrc, params NpgsqlParameter[] arrParam)
        {
            var dt = new DataTable();
            var connStr = ConfigurationManager.ConnectionStrings[connStrSettings].ConnectionString;

            Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connStr);
            Npgsql.NpgsqlCommand cmd = conn.CreateCommand();

            try
            {
                cmd.CommandText = strSrc;
                foreach (var param in arrParam)
                {
                    cmd.Parameters.Add(param);
                }
                using (NpgsqlDataAdapter ad = new NpgsqlDataAdapter(cmd)) //riempimento tabella
                {
                    ad.Fill(dt);
                }

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        // versione che gestisce transazioni ( anche la rollback deve essere fatta fuori)
        public static DataTable ExecuteReader(string strSrc, NpgsqlConnection conn, params NpgsqlParameter[] arrParam)
        {
            var dt = new DataTable();
            Npgsql.NpgsqlCommand cmd = conn.CreateCommand();

            try
            {
                cmd.CommandText = strSrc;
                foreach (var param in arrParam)
                {
                    cmd.Parameters.Add(param);
                }
                using (NpgsqlDataAdapter ad = new NpgsqlDataAdapter(cmd)) //riempimento tabella
                {
                    ad.Fill(dt);
                }

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}