using Domain;
using DomainServices.Interfaces.Repositories;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {

        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) => _context = context;
        public async Task<bool> CreateMessage(Message message)
        {
            try
            {
                _context.Message.Add(message);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
