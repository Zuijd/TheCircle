using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainServices.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class StreamRepository : IStreamRepository
    {

        private readonly string _connectionString;
        private readonly string _tableName;

        public StreamRepository(string connectionString)
        {
            _connectionString = connectionString;
            _tableName = "Stream";
        }

        public void SaveCompensation(decimal compensation, int streamId)
        {
            // Example: Storing in a database
            // Implement your code to store the log message in a database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"UPDATE {_tableName} SET Satoshi = Satoshi + @Satoshi WHERE Id = @StreamId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Satoshi", compensation);
                    command.Parameters.AddWithValue("@StreamId", streamId);


                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
