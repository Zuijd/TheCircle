namespace Portal.Controllers;

[TLSAccess]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;
    private readonly ICertificateService _certificateService;

    public HomeController(ILogger<HomeController> logger, IUserService userService, ICertificateService certificateService)
    {
        _logger = logger;
        _userService = userService;
        _certificateService = certificateService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        ViewData["PrivateKey"] = await _userService.GetSpecificClaim(User.Identity?.Name!, "PrivateKey");
        ViewData["Certificate"] = await _userService.GetSpecificClaim(User.Identity?.Name!, "Certificate");

        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult TestPost(string subject, string body, string signature, string certificate)
    {
        Console.WriteLine(subject);
        Console.WriteLine(body);
        Console.WriteLine(certificate);
        Console.WriteLine("-----------------------------");
        Console.WriteLine(signature);

        var publicKey2 = _certificateService.getPublicKeyOutOfUserCertificate(Convert.FromBase64String(certificate));

        _certificateService.VerifyDigSig("hoi", Convert.FromBase64String(signature), publicKey2);

        return RedirectToAction("Test", "Home");
    }
}
