namespace Portal.Controllers
{
    [TLSAccess]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICertificateService _certificateService;

        public UserController(IUserService userService, ICertificateService certificateService)
        {
            _userService = userService;
            _certificateService = certificateService;
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
