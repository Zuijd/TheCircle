using System.Diagnostics;
using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using Microsoft.Extensions.Logging;
using DomainServices.Interfaces.Services;

namespace Portal.Controllers;

[TLSAccess]
public class HomeController : Controller
{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerService _logger;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

    public HomeController(IHttpContextAccessor httpContextAccessor, ILoggerService logger, IUserService userService, IMessageService messageService)
    {
        _httpContextAccessor = httpContextAccessor;
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

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
