using System.Diagnostics;
using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using Microsoft.Extensions.Logging;
using DomainServices.Interfaces.Services;

namespace Portal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;

    public HomeController(ILogger<HomeController> logger, IUserService userService, IMessageService messageService)
    {
        _logger = logger;
        _userService = userService;
        _messageService = messageService;
    }

    public IActionResult Index()
    {
        string username = User.Identity?.Name!; // Retrieve the username from the user identity
        ViewBag.Username = username; // Pass the username to the ViewBag

        var users = _userService.GetAllUsers().Result;
        ViewBag.Users = users;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
