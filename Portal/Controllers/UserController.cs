using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Portal.Models.User;

namespace Portal.Controllers
{
    [TLSAccess]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, ILoggerService logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [PreventAccess]
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
                        await _logger.Log(loginViewModel.Username, $"{loginViewModel.Username} has logged in!");
                        
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

        [PreventAccess]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(registerViewModel.Username!, registerViewModel.EmailAddress!, registerViewModel.Password!);

                if (result)
                {
                    //Login user
                    await _userService.LoginUserAsync(registerViewModel.Username!, registerViewModel.Password!);
                    await _logger.Log(registerViewModel.Username, $"Registered user: {registerViewModel.Username}");
                    
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
                    await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has logged out");
                    var user = await _userService.SignUserOutAsync();
                    HttpContext.Session.Remove("Username");
                }
            }
            catch (KeyException e)
            {
                ModelState.AddModelError(e.Key, e.Message);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AddSatoshi([FromBody] dynamic satoshi)
        {
            try
            {
                var succes = await this._userService.AddSatoshi(satoshi);
                await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} trying to add balance to his account");
                return Ok(succes);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
                return BadRequest(e.Message);
            }
        }


    }
}
