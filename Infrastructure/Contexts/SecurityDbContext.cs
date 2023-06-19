namespace Infrastructure.Contexts
{
    public class SecurityDbContext : IdentityDbContext<UserIdentity>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options) { }
    }
}
