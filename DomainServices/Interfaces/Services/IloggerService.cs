using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Services
{
    public interface ILoggerService
    {
        public Task Log(string user, string message);
        public Task<List<Log>> GetAll();
        public Task<PKC> GetAllFromUsername(string username, byte[] signature, byte[] certificate);

    }
}
