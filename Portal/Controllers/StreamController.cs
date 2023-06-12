using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

public class StreamController : Controller
{
    private readonly ILogger<StreamController> _logger;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public StreamController(ILogger<StreamController> logger, IMessageService messageService, IUserService userService)
    {
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Watch()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Message([FromBody] ChatViewModel chatViewModel)
    {

        Debug.WriteLine("dedede1");


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

            }
        }
        catch (Exception e)
        {

            ModelState.AddModelError(e.Message, e.Message);
        }

        return PartialView("_Chat");
    }
}