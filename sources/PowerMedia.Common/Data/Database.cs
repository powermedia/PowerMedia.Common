using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace PowerMedia.Common.Data
{
    public class Database
    {


        /// <summary>
        /// Dodanie parametru zapytania bazodanowego
        /// </summary>
        /// <param name="command">obiekt zapytania do którego ma zostać dodany parametr</param>
        /// <param name="name">nazwa parametru poprzedzona znakiem @</param>
        /// <param name="value">wartość parametru</param>
        static public void AddParameter(IDbCommand command, string name, object value)
        {
            IDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;

            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
            command.Parameters.Add(parameter);
        }



        public static DataRowCollection GetRowsForSqlQuery(
           string connectionString, 
           string queryWithBindings,
           Dictionary<string, object> bindings)
        {
            DataSet result = new DataSet();
            IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryWithBindings;
            foreach (string key in bindings.Keys)
            {
                AddParameter(command, String.Format("@{0}", key), bindings[key]);
            }
            using (IDataReader reader = command.ExecuteReader())
            {
                result.Load(reader, LoadOption.OverwriteChanges, "table");
            }
                
            connection.Close();
            
            DataTable table = result.Tables["table"];
            return table.Rows;
        }

        public static T ExecuteScalar<T>(string connectionString, string queryWithBindings,
           Dictionary<string, object> bindings)
        {
            IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryWithBindings;
            foreach (string key in bindings.Keys)
            {
                AddParameter(command, String.Format("@{0}", key), bindings[key]);
            }
            object objectTemp = command.ExecuteScalar();
            connection.Close();
            T result = (T)objectTemp;
            return result;
        }

        public static int ExecuteNonQuery(string connectionString, string statementWithBindings,
            Dictionary<string, object> bindings)
        {
            IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = statementWithBindings;
            foreach (string key in bindings.Keys)
            {
                AddParameter(command, String.Format("@{0}", key), bindings[key]);
            }
            int rows = command.ExecuteNonQuery();
            connection.Close();
            return rows;
        }

    }
}
