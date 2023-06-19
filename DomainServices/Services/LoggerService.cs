using DomainServices.Interfaces.Repositories;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Http;
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
            _loggerRepository.addLog(new Log(_user, message));
        }
        
        public async Task<List<Log>> GetAll() {
            return await _loggerRepository.GetAll();
        }

        public async Task<List<Log>> GetAllFromUsername(string username)
        {
            return await _loggerRepository.GetAllFromUsername(username);
        }

        public string GetUserNameFromSession() 
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
