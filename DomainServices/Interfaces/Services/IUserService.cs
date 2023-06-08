namespace DomainServices.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> LoginUserAsync(string username, string password);
        Task<bool> RegisterUserAsync(string username, string emailAddress, string password);
        Task<bool> SignUserOutAsync();
    }
}
