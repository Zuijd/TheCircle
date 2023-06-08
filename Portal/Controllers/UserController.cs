namespace Portal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
            }

            return RedirectToAction("Index", "Home");
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
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Message, e.Message);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Console.WriteLine("WAT");
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
