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
    public class LoggerRepository : ILoggerRepository
    {

        private readonly ApplicationDbContext _context;

        public LoggerRepository(ApplicationDbContext context) => _context = context;

        public async Task<bool> addLog(Log log) {
            try
            {
                Console.WriteLine(log.ToString());
                
                await _context.Log.AddAsync(log);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
