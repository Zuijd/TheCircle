using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Logger
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DatabaseLoggerProvider(string connectionString, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new DatabaseLogger(_connectionString);
            logger.SetUserName(GetUserNameFromIdentity());
            return logger;
        }

        public void Dispose()
        {

        }

        private string GetUserNameFromIdentity()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            return userName ?? "System";
        }
    }
}
