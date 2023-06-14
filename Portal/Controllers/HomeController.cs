using System.Diagnostics;
using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

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
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Message([FromBody] ChatViewModel chatViewModel)
    {
        try
        {

            if (ModelState.IsValid)
            {
                Console.WriteLine(chatViewModel.Username);
                Message message = new()
                {
                    User = _userService.GetUserByName(User.Identity?.Name!).Result,
                    MessageBody = chatViewModel.Message!
                };


                await _messageService.CreateMessage(message);

            }
        }
        catch (Exception e)
        {

            ModelState.AddModelError(e.Message, e.Message);
        }

        return View("Index");
    }
}
