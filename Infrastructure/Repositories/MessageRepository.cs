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
                await _context.Message.AddAsync(message);
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
