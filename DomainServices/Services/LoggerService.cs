﻿using DomainServices.Interfaces.Repositories;
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
    public class LoggerService : ILoggerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository _loggerRepository;

        private string _user;

        public LoggerService(IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _loggerRepository = loggerRepository;
            _user = GetUserNameFromSession();
        }

        public void Log(string message)
        {
            DateTime timestamp = DateTime.UtcNow.AddHours(2);
            _loggerRepository.addLog(new Log { Username = _user, Message = message, Timestamp = timestamp });
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
    }
}
