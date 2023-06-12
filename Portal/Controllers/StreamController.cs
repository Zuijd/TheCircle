using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

public class StreamController : Controller
{
    private readonly ILogger<StreamController> _logger;
    private readonly IMessageService _messageService;

    public StreamController(ILogger<StreamController> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
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
        try
        {

            if (ModelState.IsValid)
            {
                Message message = new()
                {
                    Username = chatViewModel.Username!,
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