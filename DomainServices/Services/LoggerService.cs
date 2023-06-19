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
using System.Reflection.Metadata;
using System.Security.Cryptography.Xml;

namespace DomainServices.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository _loggerRepository;
        private readonly ICertificateService _certificateService;
        private string _user;

        public LoggerService(IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository, ICertificateService certificateService)
        {
            _httpContextAccessor = httpContextAccessor;
            _loggerRepository = loggerRepository;
            _certificateService = certificateService;
            _user = GetUserNameFromSession();
        }

        public void Log(string message)
        {
            _loggerRepository.addLog(new Log(_user, message));
        }
        
        public async Task<List<Log>> GetAll() {
            return await _loggerRepository.GetAll();
        }

        public async Task<PKC> GetAllFromUsername(string username, byte[] signature, byte[] certificate)
        {
            var publicKey = _certificateService.GetPublicKeyOutOfCertificate(certificate);

            //verify digital signature
            var isValid = _certificateService.VerifyDigSig(username, signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "CLIENT PACKET IS VALID" : "CLIENT PACKET IS INVALID");

            var content = await _loggerRepository.GetAllFromUsername(username);

            return new PKC()
            {
                Message = content,
                Signature = _certificateService.CreateDigSig(content, _certificateService.GetPrivateKeyFromServer()),
                Certificate = _certificateService.GetCertificateFromServer(),
            };
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
