using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using Dapper;

namespace SindicatosWorker.Aaspa.Repository.Mdb
{
    public class MdbContext
    {
        private readonly OdbcConnection connection = null;
        private readonly string _databasePath;

        public MdbContext(string databasePath)
        {
            _databasePath = databasePath;

            string connectionString = $"Driver={{Microsoft Access Driver (*.mdb)}};Dbq={_databasePath};";

            connection = new OdbcConnection(connectionString);
        }

        private void CheckConnectionIsOpen()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
        }

        public List<T> Query<T>(string query)
        {
            try
            {
                CheckConnectionIsOpen();
                return connection.Query<T>(query).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> Tables()
        {
            CheckConnectionIsOpen();
            var list = new List<string>();
        
            var rows = connection.GetSchema("tables");
            for (int i = 0; i < rows.Rows.Count; i++)
            {
                if (rows.Rows[i]["TABLE_TYPE"].ToString() == "TABLE")
                {
                    list.Add(rows.Rows[i]["TABLE_NAME"].ToString());
                }
            }
            return list;
        }

    }
}
