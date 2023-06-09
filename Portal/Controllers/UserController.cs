using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserService userService)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [PreventAccessFilter]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.LoginUserAsync(loginViewModel.Username!, loginViewModel.Password!);

                    if(user)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(loginViewModel.Password), e.Message);
            }

            return View(loginViewModel);
        }

        [PreventAccessFilter]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (registerViewModel.Username != null)
            {
                if(registerViewModel.Username.Length < 6)
                    ModelState.AddModelError("Username", "The username should be a at least 6 characters long!");
                
                else
                {
                    var existingUser = await _userManager.FindByNameAsync(registerViewModel.Username);

                    if (existingUser != null)
                    ModelState.AddModelError("Username", "The username is already associated with an account!");
                }
            }

            if (registerViewModel.Password != null)
                if (!PasswordValidation(registerViewModel.Password))
                   ModelState.AddModelError("Password", "The password should be at least 8 characters long and contrain at least 1 uppercase letter and 1 number!");

            if (registerViewModel.EmailAddress != null)
            {
                if (!EmailValidation(registerViewModel.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "The emailaddress is not valid!");
                }
                else
                {
                    var existingUser = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

                    if (existingUser != null)
                    ModelState.AddModelError("EmailAddress", "The emailaddress is already associated with an account!");
                }
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.EmailAddress
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(user, registerViewModel.Password, false, false);
                    return RedirectToAction("Index", "Home");
                }
                return View(registerViewModel);
            }
            return View(registerViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.SignUserOutAsync();
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
            }

            return RedirectToAction("Index", "Home");
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
