using DomainServices.Logger;
using Microsoft.Extensions.Logging;
using Portal.Models.User;

namespace Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;  // Use ILogger<UserController> instead of DatabaseLogger
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, ILogger<UserController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.LoginUserAsync(loginViewModel.Username!, loginViewModel.Password!);

                    if (user)
                    {
                        HttpContext.Session.SetString("Username", loginViewModel.Username!);
                        using (_logger.BeginScope(loginViewModel.Username!))  // Use _logger instead of _databaseLogger
                        {
                            _logger.LogInformation($"{loginViewModel.Username} has logged in!");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
            }

            return View(loginViewModel);
        }



        public IActionResult Register() => View();


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.RegisterUserAsync(registerViewModel.Username!, registerViewModel.EmailAddress!, registerViewModel.Password!);

                    if (user)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
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
                    HttpContext.Session.Remove("Username"); // Clear the username from the session

                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
