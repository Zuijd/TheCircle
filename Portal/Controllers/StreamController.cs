using System.Diagnostics;
using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

public class StreamController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IloggerService _logger;

    public StreamController(IloggerService logger, IMessageService messageService, IUserService userService)
    {
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        _logger.Info("User has accessed Stream page!");
        return View();
    }

    public IActionResult Watch()
    {
        _logger.Info($"User has accessed {nameof(Watch)}");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Message([FromBody] ChatViewModel chatViewModel)
    {

        try
        {

            if (ModelState.IsValid)
            {

                Message message = new()
                {
                    User = await _userService.GetUserByName(User.Identity?.Name!),
                    MessageBody = chatViewModel.Message!
                };

                await _messageService.CreateMessage(message);
                _logger.Info("User created message!");

            }
        }
        catch (Exception e)
        {

            ModelState.AddModelError(e.Message, e.Message);
        }

        return PartialView("_Chat");
    }
}