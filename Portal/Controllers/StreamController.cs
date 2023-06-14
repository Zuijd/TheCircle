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
    private readonly ILoggerService _logger;
    private readonly ISatoshiCompensation _satoshiCompensationService;
    private readonly IStreamService _streamService;

    public StreamController(
        ILoggerService logger, 
        IMessageService messageService, 
        IUserService userService, 
        ISatoshiCompensation satoshiCompensationService, 
        IStreamService streamService
        )
    {
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
        _satoshiCompensationService = satoshiCompensationService;
        _streamService = streamService;
    }

    public IActionResult Index()
    {
        _logger.Log("User has accessed Stream page!");
        return View();
    }

    public IActionResult Watch()
    {
        _logger.Log($"User has accessed {nameof(Watch)}");
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
                _logger.Log("User created message!");

            }
        }
        catch (Exception e)
        {

            ModelState.AddModelError(e.Message, e.Message);
        }

        return PartialView("_Chat");
    }

    // Cals for the stream.js file to save ongoing streams
    // Add Stream
    [HttpPost]
    public async Task<IActionResult> AddStream([FromBody] Streams stream)
    {
        try
        {
            this._streamService.AddStream(stream);
            return Ok();
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }

    // Add Break
    [HttpPost]
    public async Task<IActionResult> AddBreak([FromBody] Break pauze)
    {
        try
        {
            this._streamService.AddBreakMoment(pauze);
            return Ok();
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }

    // Add Live
    [HttpPost]
    public async Task<IActionResult> AddLive([FromBody] Live live)
    {
        try
        {
            this._streamService.AddLiveMoment(live);
            return Ok();
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }

    // Add Live
    [HttpPost]
    public async Task<IActionResult> AddSatoshi([FromBody] Live live)
    {
        try
        {
            this._streamService.AddLiveMoment(live);
            return Ok();
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }



}