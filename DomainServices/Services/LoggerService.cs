using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class LoggerService : IloggerService
    {

        private readonly ILogger<LoggerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string _loggerName;

        public LoggerService(ILogger<LoggerService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _loggerName = _httpContextAccessor.HttpContext.Session.GetString("Username") ?? "System";
        }

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
