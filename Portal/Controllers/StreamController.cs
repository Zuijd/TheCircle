using System;
using System.Diagnostics;
using Domain;
using DomainServices.Interfaces;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
namespace Portal.Controllers;

public class StreamController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly ILoggerService _logger;
    private readonly IStreamService _streamService;

    public StreamController(
        ILoggerService logger,
        IMessageService messageService,
        IUserService userService,
        IStreamService streamService
        )
    {
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
        _streamService = streamService;
    }

    public IActionResult Index()
    {
        _logger.Log("User has accessed Stream page!");
        return View();
    }

    public IActionResult Watch(string id)
    {
        _logger.Log($"User has accessed {nameof(Watch)}");
        return View();
    }

    // Cals for the stream.js file to save ongoing streams
    // Add Stream
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