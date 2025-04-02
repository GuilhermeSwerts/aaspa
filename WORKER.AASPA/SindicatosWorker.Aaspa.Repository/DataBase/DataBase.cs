using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace SindicatosWorker.Aaspa.Repository.DataBase
{
    public static class DataBase
    {
        private static readonly string _connectionString;
        private static readonly MySqlConnection connection;

        static DataBase()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            connection = Dbo();
        }

        public async static Task<MySqlTransaction> BeginTransactionAsync() => await connection.BeginTransactionAsync();
        public async static Task CommitAsync(this MySqlTransaction transaction) => await transaction.CommitAsync();
        public async static Task RollbackAsync(this MySqlTransaction transaction) => await transaction.RollbackAsync();

        public static MySqlConnection Dbo()
        {
            var con = new MySqlConnection(_connectionString);
            try
            {
                con.Open();
                return con;
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public static async Task ExecuteAsync(string query, MySqlTransaction transaction = null)
        {
            try
            {
                if (transaction == null)
                    await connection.QueryAsync(query);
                else
                    await connection.QueryAsync(query,transaction: transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar a query: " + ex.Message);
            }
        }
    }
}
