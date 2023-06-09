using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DomainServices.Logger
{
    public class DatabaseLogger : ILogger
    {

        private readonly string _connectionString;
        private readonly string _tableName;

        private string _userName = string.Empty;


        public DatabaseLogger(string connectionString)
        {
            _connectionString = connectionString;
            _tableName = "Logs";

        }

        public void SetUserName(string userName)
        {
            _userName = userName;
        }
        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // implement wich levels are accepted
            // Currently all are accepeted
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // Implement your custom logging logic here
            if (!IsEnabled(logLevel))
            {
                return;
            }
            string formattedMessage = formatter(state, exception);

            // Filter messages based on custom criteria
            if (ShouldExcludeLogMessage(formattedMessage))
            {
                return;
            }

            string exceptionMessage = exception?.ToString() ?? string.Empty;

            // Example: Writing to console
            Console.WriteLine($"[{logLevel}] - {formatter(state, exception)}");

            // Example: Storing in a database
            // Implement your code to store the log message in a database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"INSERT INTO {_tableName} (Timestamp, Message, Level, Exception, Username) VALUES (@Timestamp, @Message, @Level, @Exception, @Username)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow.AddHours(2));
                    command.Parameters.AddWithValue("@Message", formattedMessage);
                    command.Parameters.AddWithValue("@Level", logLevel.ToString());
                    command.Parameters.AddWithValue("@Exception", exceptionMessage);
                    command.Parameters.AddWithValue("@Username", _userName);


                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

        }


        private bool ShouldExcludeLogMessage(string logMessage)
        {
            // Add your custom filtering logic here
            string[] excludedKeywords = { "listening on", "application started", "Hosting environment", "Content root path", "Entity Framework", "An error", "An exception" };
            foreach (string keyword in excludedKeywords)
            {
                if (logMessage.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true; // Exclude the log message
                }
            }

            return false; // Include the log message
        }   
    }
}
