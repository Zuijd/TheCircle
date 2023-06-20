namespace Portal.Controllers;

[TLSAccess]
public class HomeController : Controller
{
    private readonly ILoggerService _logger;
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;

    public HomeController(ILoggerService logger, IUserService userService, IMessageService messageService)
    {
        _logger = logger;
        _userService = userService;
        _messageService = messageService;
    }

    public async Task<IActionResult> Index()
    {
        string username = User.Identity?.Name!; // Retrieve the username from the user identity
        ViewBag.Username = username; // Pass the username to the ViewBag

        var users = await _userService.GetAllUsers();
        ViewBag.Users = users;

        if(User.Identity.IsAuthenticated)
        {
            _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Home page!");
        }

        return View();
    }

    [Authorize]
    public async Task<IActionResult> Watch()
    {
        string username = User.Identity?.Name!; // Retrieve the username from the user identity
        ViewBag.Username = username; // Pass the username to the ViewBag

        var users = await _userService.GetAllUsers();
        ViewBag.Users = users;

        _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Watch page to choose a stream!");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
