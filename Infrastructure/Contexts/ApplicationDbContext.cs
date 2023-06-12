using Domain;

namespace Infrastructure.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        //users
        public DbSet<Message> Message { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
