using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class DatabaseLogger : ILogger
{
    private readonly string _connectionString;
    private readonly string _tableName;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseLogger(string connectionString, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = connectionString;
        _tableName = "Logs";
        _httpContextAccessor = httpContextAccessor;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string formattedMessage = formatter(state, exception);

        if (ShouldExcludeLogMessage(formattedMessage))
        {
            return;
        }

        string exceptionMessage = exception?.ToString() ?? string.Empty;

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
                command.Parameters.AddWithValue("@Username", GetUserNameFromSession());

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    private string GetUserNameFromSession()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var session = httpContext.Session;
            if (session != null)
            {
                var userName = session.GetString("Username");
                if (!string.IsNullOrEmpty(userName))
                {
                    return userName;
                }
            }
        }

        return "System";
    }

    private bool ShouldExcludeLogMessage(string logMessage)
    {
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
