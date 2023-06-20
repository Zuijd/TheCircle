using DomainServices.Interfaces.Repositories;

namespace DomainServices.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly ICertificateService _certificateService;

        public UserService(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager, IUserRepository userRepository, ICertificateService certificateService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _certificateService = certificateService;
        }
        
        public async Task<UserIdentity> GetUser(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<bool> LoginUserAsync(string username, string password)
        {
            var exceptions = new List<Exception>();

            if (username == null)
            {
                exceptions.Add(new KeyException("Username", "Username is required!"));
            }

            if (password == null)
            {
                exceptions.Add(new KeyException("Password", "Password is required!"));
            }

            if (exceptions.Count == 0)
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user != null)
                {
                    if ((await _signInManager.PasswordSignInAsync(user, password, false, false)).Succeeded)
                    {
                        return true;
                    }
                }

                exceptions.Add(new KeyException("Password", "Incorrect username and password combination."));
            }

            throw new MultipleExceptions(exceptions);
        }

        public async Task<bool> RegisterUserAsync(string username, string emailAddress, string password)
        {
            var exceptions = new List<Exception>();

            if (username == null)
            {
                exceptions.Add(new KeyException("Username", "Username is required!"));
            }
            else
            {
                if (username.Length < 6)
                {
                    exceptions.Add(new KeyException("Username", "The username should be a at least 6 characters long!"));
                }
                else
                {
                    var userExistsUsername = await _userManager.FindByNameAsync(username);

                    if (userExistsUsername != null)
                    {
                        exceptions.Add(new KeyException("Username", $"Username '{username}' is already taken. Please choose a different username."));
                    }
                }
            }

            if (emailAddress == null)
            {
                exceptions.Add(new KeyException("EmailAddress", "Email address is required!"));
            }
            else
            {
                if (!EmailValidation(emailAddress))
                {
                    exceptions.Add(new KeyException("EmailAddress", "The emailaddress is not valid!"));
                }
                else
                {
                    var userExistsEmailAddress = await _userManager.FindByEmailAsync(emailAddress);

                    if (userExistsEmailAddress != null)
                    {
                        exceptions.Add(new KeyException("EmailAddress", $"Email address '{emailAddress}' is already taken. Please choose a different email address."));
                    }
                }
            }

            if (password == null)
            {
                exceptions.Add(new KeyException("Password", "Password is required!"));
            }
            else
            {
                if (!PasswordValidation(password))
                {
                    exceptions.Add(new KeyException("Password", "The password should be at least 8 characters long and contrain at least 1 uppercase letter and 1 number!"));
                }
            }

            if (exceptions.Count == 0)
            {
                var keyPair = _certificateService.CreateKeyPair();
                var certificate = _certificateService.CreateCertificate(username!, emailAddress!, keyPair!);

                var user = new UserIdentity
                {
                    UserName = username,
                    Email = emailAddress,
                    EmailConfirmed = true,
                    Certificate = certificate,
                    PrivateKey = _certificateService.GetPrivateKey(keyPair),
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userRepository.CreateUser(new User { Name = username, Email = emailAddress });
                }

                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim("PrivateKey", Convert.ToBase64String(_certificateService.GetPrivateKey(keyPair))));
                    await _userManager.AddClaimAsync(user, new Claim("PublicKey", Convert.ToBase64String(_certificateService.GetPublicKeyOutOfCertificate(certificate))));
                    await _userManager.AddClaimAsync(user, new Claim("Certificate", Convert.ToBase64String(certificate)));
                }

                return result.Succeeded;
            }

            throw new MultipleExceptions(exceptions);
        }

        public async Task<bool> SignUserOutAsync()
        {
            await _signInManager.SignOutAsync();

            if (_signInManager.Context.User.Identity!.IsAuthenticated)
            {
                return true;
            }

            throw new KeyException("SignOut", "Failed to sign out.");
        }

        public bool PasswordValidation(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
        }

        public bool EmailValidation(string email)
        {
            var mailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return mailRegex.IsMatch(email);
        }

        public async Task<User> GetUserByName(string username) => await _userRepository.GetUserByName(username);
        
        public async Task<List<User>> GetAllUsers() => await _userRepository.GetAllUsers();

        public async Task<string> GetSpecificClaim(string username, string claimType)
        {
            var user = await _userManager.FindByNameAsync(username);

            var claims = await _userManager.GetClaimsAsync(user);

            var claim = claims.FirstOrDefault(claim => claim.Type.Equals(claimType));

            if (claim != null)
            {
                return claim.Value;
            }

            throw new KeyException($"No{claimType}Claim", $"No {claimType} claim present");
        }

    }
}
