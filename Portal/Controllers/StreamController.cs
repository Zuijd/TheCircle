using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Domain;
using DomainServices.Interfaces;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
namespace Portal.Controllers;

[TLSAccess]
public class StreamController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly ILoggerService _logger;
    private readonly IStreamService _streamService;

    public StreamController(IHttpContextAccessor httpContextAccessor, ILoggerService logger, IMessageService messageService, IUserService userService, IStreamService streamService)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
        _streamService = streamService;
    }

    [Authorize]
    public IActionResult Index()
    {
        _logger.Log("User has accessed Stream page!");

        ViewBag.UserName = User.Identity?.Name!; 
        return View();
    }

    [Authorize]
    public IActionResult Watch(string id)
    {
        
        if(id == "404")
        {
            return View("404");
        }
        
        _logger.Log($"User has accessed {nameof(Watch)}");

        ViewBag.UserName = User.Identity?.Name!;
        return View();
    }

    // Cals for the stream.js file to save ongoing streams
    // Add Stream
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddStream([FromBody] dynamic newStreamInfo)
    {
        try
        {
            var streamId = await this._streamService.AddStream(newStreamInfo);
            _logger.Log("User started a stream!");
            return Ok(streamId);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }

    // Stop Stream
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> StopStream([FromBody] dynamic stopStreamInfo)
    {
        try
        {
            var succes = await this._streamService.StopStream(stopStreamInfo);
            _logger.Log("User ended a stream!");
            return Ok(succes);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }

    // Add Break
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddBreak([FromBody] dynamic pauze)
    {
        try
        {
            var succes = await this._streamService.AddBreakMoment(pauze);
            _logger.Log("User is back live after a break!");
            return Ok(succes);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }

    // Add Live
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddLive([FromBody] dynamic live)
    {
        try
        {
            var succes = await this._streamService.AddLiveMoment(live);
            _logger.Log("User started a break and is no longer live!");
            return Ok(succes);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
            return BadRequest(e.Message);
        }
    }
}