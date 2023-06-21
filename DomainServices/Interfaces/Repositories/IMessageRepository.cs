using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        public Task<bool> CreateMessage(Message message); 
    }
}
