using DomainServices.Logger;
using Portal.Models.User;

namespace Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly ILoggerProvider _loggerProvider;


        public UserController(IUserService userService, ILogger<UserController> logger, ILoggerProvider loggerProvider)
        {
            _userService = userService;
            _logger = logger;
            _loggerProvider = loggerProvider;
          
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

                    if(user)
                    {
                    
                        _logger.LogInformation($"{loginViewModel.Username} has logged in!");
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
