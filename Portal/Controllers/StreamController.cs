using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using Domain;
using DomainServices.Interfaces;
using DomainServices.Interfaces.Services;
using DomainServices.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Portal.Models;

namespace Portal.Controllers;

[TLSAccess]
public class StreamController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly ILoggerService _logger;
    private readonly IStreamService _streamService;
    private readonly ICertificateService _certificateService;

    public StreamController(ILoggerService logger, IMessageService messageService, IUserService userService, IStreamService streamService, ICertificateService certificateService)
    {
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
        _streamService = streamService;
        _certificateService = certificateService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Stream page!");

        ViewBag.UserName = User.Identity?.Name!;
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Watch(string id)
    {

        if (id == "404")
        {
            return View("404");
        }
        
        await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Watch page of a stream!");

        ViewBag.UserName = User.Identity?.Name!;
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> History()
    {
        try
        {
            var streams = await _streamService.GetStreams();
            await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Stream History page!");
            return View(streams);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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
            await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} started a stream!");
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
            await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} ended a stream!");
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
       
            var succes = await this._streamService.AddBreakMoment(pauze);
            await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} is back live after a break!");
            return Ok(succes);
       
    }

    // Add Live
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddLive([FromBody] dynamic live)
    {
        
            var succes = await this._streamService.AddLiveMoment(live);
            await _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} started a break and is no longer live!");
            return Ok(succes);
      
    }

    [HttpPost]
    public async Task<bool> SaveChunk()
    {
        using (var stream = new MemoryStream())
        {
            await Request.Body.CopyToAsync(stream);
            byte[] chunk = stream.ToArray();

            await _streamService.SaveChunk(chunk);

            ///// * CREATE DIGSIG FOR CREATEPOST (SERVICE) * /////
            //retrieve private key
            var privateKey = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "PrivateKey"));

            //retrieve certificate
            var certificate = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "Certificate"));

            //create digital signature
            var digSig = _certificateService.CreateDigSig(chunk, privateKey);

            //call request to service
            var serverResponse = _streamService.ValidateChunk(chunk, digSig, certificate);

            ///// * VERIFY REQUEST FROM CREATEPOST * /////
            //retrieve public key from certificate
            var publicKey = _certificateService.GetPublicKeyOutOfCertificate(serverResponse.Certificate);

            //verify digital signature
            var isValid = _certificateService.VerifyDigSig(serverResponse.Message, serverResponse.Signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "STREAM - SERVER PACKET IS VALID" : "STREAM - SERVER PACKET IS INVALID");

            return isValid;
        }
    }
}
