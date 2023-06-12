using DomainServices.Interfaces.Services;
using DomainServices.Logger;
using Microsoft.Extensions.Logging;
using Portal.Models.User;

namespace Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IloggerService _logger;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor, IloggerService logger)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [PreventAccessFilter]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                var user = await _userService.LoginUserAsync(loginViewModel.Username!, loginViewModel.Password!);

                    if (user)
                    {
                        HttpContext.Session.SetString("Username", loginViewModel.Username!);
                        _logger.Info("User has logged in!");
                        
                        return RedirectToAction("Index", "Home");
                    }
            }
            catch (MultipleExceptions e)
            {
                foreach (var innerExc in e.InnerExceptions)
                {
                    ModelState.AddModelError((((KeyException)innerExc).Key), (((KeyException)innerExc).Message));
                }
            }

            return View(loginViewModel);
        }

        [PreventAccessFilter]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(registerViewModel.Username!, registerViewModel.EmailAddress!, registerViewModel.Password!);

                if (result)
                {
                    await _userService.LoginUserAsync(registerViewModel.Username!, registerViewModel.Password!);
                    
                    return RedirectToAction("Index", "Home");
                }
            } catch (MultipleExceptions e)
            {
                foreach (var innerExc in e.InnerExceptions)
                {
                    ModelState.AddModelError((((KeyException) innerExc).Key), (((KeyException) innerExc).Message));
                }
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
                    _logger.Info("User has logged out");
                    HttpContext.Session.Remove("Username"); 
                 
                }
            }
            catch (KeyException e)
            {
                ModelState.AddModelError(e.Key, e.Message);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
