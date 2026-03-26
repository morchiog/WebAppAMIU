using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;

namespace WebAppAMIU.DbConnector
{
    public class MOracleHelp
    {
        private OracleConnection conn;
        internal OracleConnection Connection { get { return conn; } }
        internal MOracleHelp(string connectionStringKey)
        {
            if (ConfigurationManager.ConnectionStrings[connectionStringKey] != null)
            {
                conn = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString);
                conn.Open();
            }
        }

        internal OracleCommand CreateCommand(string sqlCommandText, OracleTransaction transaction = null)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommandText;
                return cmd;
            }
            return null;
        }


        public static string ManageDateOrNullVal(string v)
        {
            if (string.IsNullOrWhiteSpace(v?.Trim())) { return "null"; }
            ;

            v = "to_date('" + v + "','ddmmyyyy')";

            return v.Trim();
        }

        public static string ManageStringOrNullVal(string v)
        {
            if (string.IsNullOrWhiteSpace(v?.Trim())) { return "null"; }
            ;

            v = v.Replace("'", "''");

            return "'" + v.Trim() + "'";
        }

        public static string ManageIntOrNullVal(string v)
        {
            if (v == null) { return "null"; }
            ;

            return v.ToString();
        }

        public static int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            var conn = new OracleConnection(ConfigurationManager.ConnectionStrings["FOTIDWH"].ConnectionString);
            conn.Open();
            try
            {
                return ExecuteNonQuery(conn, commandType, commandText, null);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }



        }


        public static int ExecuteNonQuery(Oracle.ManagedDataAccess.Client.OracleConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }
        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
            return cmd.ExecuteNonQuery();
        }

        public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
            return cmd.ExecuteNonQuery();
        }

        public static DataTable ExecuteReaderUO(OracleConnection conn, string commandText, params OracleParameter[] arrParam)
        {
            var isInternal = false;
            try
            {
                if (conn == null)
                {
                    isInternal = true;
                    conn = new OracleConnection(ConfigurationManager.ConnectionStrings["UO"].ConnectionString);
                    conn.Open();
                }

                var dt = new DataTable();
                OracleCommand cmd = conn.CreateCommand();
                if (arrParam != null)
                {
                    foreach (var param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                cmd.CommandText = commandText;
                cmd.BindByName = true;

                using (OracleDataAdapter ad = new OracleDataAdapter(cmd)) //riempimento tabella
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
                if (isInternal) { conn.Close(); }
            }
        }

        public static DataTable ExecuteReaderEcos(OracleConnection conn, string commandText, params OracleParameter[] arrParam)
        {
            var isInternal = false;
            try
            {
                if (conn == null)
                {
                    isInternal = true;
                    conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ECOS"].ConnectionString);
                    conn.Open();
                }

                var dt = new DataTable();
                OracleCommand cmd = conn.CreateCommand();
                if (arrParam != null)
                {
                    foreach (var param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                cmd.CommandText = commandText;
                cmd.BindByName = true;

                using (OracleDataAdapter ad = new OracleDataAdapter(cmd)) //riempimento tabella
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
                if (isInternal) { conn.Close(); }
            }
        }

        public static DataTable ExecuteReader(OracleConnection conn, string commandText, params OracleParameter[] arrParam)
        {
            var isInternal = false;
            try
            {
                if (conn == null)
                {
                    isInternal = true;
                    conn = new OracleConnection(ConfigurationManager.ConnectionStrings["FOTIDWH"].ConnectionString);
                    conn.Open();
                }

                var dt = new DataTable();
                OracleCommand cmd = conn.CreateCommand();
                if (arrParam != null)
                {
                    foreach (var param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                cmd.CommandText = commandText;
                cmd.BindByName = true;

                using (OracleDataAdapter ad = new OracleDataAdapter(cmd)) //riempimento tabella
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
                if (isInternal) { conn.Close(); }
            }
        }

        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            command.Connection = connection;
            command.CommandText = commandText;

            if (transaction != null)
                command.Transaction = transaction;

            command.CommandType = commandType;

            if (commandParameters != null)
                AttachParameters(command, commandParameters);

            return;
        }


        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter p in commandParameters)
            {
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    p.Value = DBNull.Value;

                command.Parameters.Add(p);
            }
        }
       



    }
}