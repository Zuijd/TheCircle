﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
        private readonly string _tableName;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseLogger(string applicationConnectionString, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = applicationConnectionString;
            _tableName = "Logs";
            _httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new DatabaseLogger(_connectionString, _httpContextAccessor);
            return logger;
        }

        public void Dispose()
        {
        }
    }
}
