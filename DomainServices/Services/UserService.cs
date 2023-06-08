namespace DomainServices.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<bool> LoginUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                if ((await _signInManager.PasswordSignInAsync(user, password, false, false)).Succeeded)
                {
                    return true;
                }

                throw new InvalidOperationException("Incorrect password. Please try again.");
            }
            else
            {
                throw new KeyNotFoundException("The username you provided does not match any registered user.");
            }
        }

        public async Task<bool> RegisterUserAsync(string username, string emailAddress, string password)
        {
            var userExistsUsername = await _userManager.FindByNameAsync(username);
            var userExistsEmailAddress = await _userManager.FindByEmailAsync(emailAddress);

            if (userExistsUsername == null && userExistsEmailAddress == null)
            {
                var user = new IdentityUser
                {
                    UserName = username,
                    Email = emailAddress,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, password);

                return result.Succeeded;
            }
            else if (userExistsUsername != null)
            {
                throw new InvalidOperationException($"Username '{username}' is already taken. Please choose a different username.");
            } else
            {
                throw new InvalidOperationException($"Email address '{emailAddress}' is already taken. Please choose a different email address.");
            }
        }

        public async Task<bool> SignUserOutAsync()
        {
            await _signInManager.SignOutAsync();

            return (_signInManager.Context.User.Identity!.IsAuthenticated) ? true : throw new InvalidOperationException("Failed to sign out.");
        }
    }
}
