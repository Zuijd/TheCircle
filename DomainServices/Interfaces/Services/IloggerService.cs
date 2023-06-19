using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Services
{
    public interface ILoggerService
    {
        public void Log(string message);
        public string GetUserNameFromSession();

        public Task<List<Log>> GetAll();
        public Task<PKC> GetAllFromUsername(string username, byte[] signature, byte[] certificate);

    }
}
