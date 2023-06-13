using DomainServices.Interfaces.Repositories;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using System.Net.Mail;

namespace DomainServices.Services
{
    public class LoggerService : IloggerService, ILogger
    {

        private readonly ILogger<LoggerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository _loggerRepository;

        private string _loggerName;

        public LoggerService(ILogger<LoggerService> logger, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _loggerRepository = loggerRepository;
            _loggerName = GetUserNameFromSession();
        }

        // Catch all logs
        public IDisposable BeginScope<TState>(TState state)
        {
            //throw new NotImplementedException();
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            DateTime timestamp = DateTime.UtcNow.AddHours(2);

            string formattedMessage = formatter(state, exception);

            if (ShouldExcludeLogMessage(formattedMessage))
            {
                return;
            }

            _loggerRepository.addLog(new Log { Username = _loggerName, Message = formattedMessage, Level = logLevel.ToString(), Timestamp = timestamp, Exception = exception?.ToString() });
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
            string[] excludedKeywords = { "listening on", "application started", "Hosting environment", "Content root path", "Entity Framework", "An error", "An exception", "Executed DbCommand" };
            foreach (string keyword in excludedKeywords)
            {
                if (logMessage.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true; // Exclude the log message
                }
            }

            return false; // Include the log message
        }

        // Processes all logs
        public void Debug(string message, params object[] args)
        {
            _logger.LogDebug(message);
        }

        public void Error(string message, params object[] args)
        {
            _logger.LogError(message);
        }

        public void Fatal(string message, params object[] args)
        {
            _logger.LogCritical(message);
        }

        public void Info(string message, params object[] args)
        {
            _logger.LogInformation(message);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.LogTrace(message);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.LogWarning(message);
        }
    }
}
