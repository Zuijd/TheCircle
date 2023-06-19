using Domain;

namespace Infrastructure.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Message> Message { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
        public DbSet<Log> Log { get; set; } = null!;

        public DbSet<Domain.Streams> Streams { get; set; } = null!;


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
