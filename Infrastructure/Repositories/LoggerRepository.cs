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

        public async Task<List<Log>> GetAll()
        {
            return await _context.Log
                .OrderByDescending(u => u.Timestamp)
                .ToListAsync();
        }

        public async Task<List<Log>> GetAllFromUsername(string username)
        {
            return await _context.Log
                .Where(u => u.Username.Equals(username))
                .OrderByDescending(u => u.Timestamp)
                .ToListAsync();
        }
    }
}
