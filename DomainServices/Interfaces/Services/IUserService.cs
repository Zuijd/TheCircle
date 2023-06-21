namespace DomainServices.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserIdentity> GetUser(string name);
        Task<bool> LoginUserAsync(string username, string password);
        Task<bool> RegisterUserAsync(string username, string emailAddress, string password);
        Task<bool> SignUserOutAsync();
        Task<string> GetSpecificClaim(string username, string claimType);
        Task<User> GetUserByName(string username);
        Task<bool> AddSatoshi(dynamic satoshi);
        Task<List<User>> GetAllUsers();

    }
}
