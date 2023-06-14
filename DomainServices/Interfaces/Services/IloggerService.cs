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
        // private string GetUserNameFromSession();
    }
}
