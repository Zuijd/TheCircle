using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetUserByName(string username);
        public Task<bool> CreateUser(User user);
        public Task<bool> AddSatoshi(string username, decimal satoshi);
    }
}
