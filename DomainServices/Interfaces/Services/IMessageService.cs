using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Services
{
    public interface IMessageService
    {
        public Task<bool> CreateMessage(Message message);
    }
}
