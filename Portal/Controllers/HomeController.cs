using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

[TLSAccess]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
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
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult TestPost(string subject, string body, string signature)
    {
        Console.WriteLine(subject);
        Console.WriteLine(body);
        Console.WriteLine(signature);

        return RedirectToAction("Index", "Home");
    }
}
