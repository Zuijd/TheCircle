using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Services
{
    public interface IloggerService
    {
        public void Info(string message, params object[] args);
        public void Error(string message, params object[] args);
        public void Warn(string message, params object[] args);
        public void Fatal(string message, params object[] args);
        public void Trace(string message, params object[] args);
        public void Debug(string message, params object[] args);

       
    }
}
