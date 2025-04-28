using Npgsql;
using System;


namespace efilling_api.Models
{
    public class DbConnectionChecker
    {
        private string _connectionString;

        public DbConnectionChecker(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open(); // This will try to open a connection to the database
                    Console.WriteLine("Database connection successful.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }
    }
}
