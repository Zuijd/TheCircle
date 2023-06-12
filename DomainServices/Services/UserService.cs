namespace DomainServices.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            } else
            {
                if (username.Length < 6)
                {
                    exceptions.Add(new KeyException("Username", "The username should be a at least 6 characters long!"));
                } else
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
            } else
            {
                if (!EmailValidation(emailAddress))
                {
                    exceptions.Add(new KeyException("EmailAddress", "The emailaddress is not valid!"));
                } else
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
            } else
            {
                if (!PasswordValidation(password))
                {
                    exceptions.Add(new KeyException("Password", "The password should be at least 8 characters long and contrain at least 1 uppercase letter and 1 number!"));
                }
            }

            if (exceptions.Count == 0)
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
    }
}
