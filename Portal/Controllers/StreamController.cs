using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

public class StreamController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly ILoggerService _logger;

    public StreamController(IHttpContextAccessor httpContextAccessor, ILoggerService logger, IMessageService messageService, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _messageService = messageService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        _logger.Log("User has accessed Stream page!");

        ViewBag.UserName = User.Identity?.Name!; 
        return View();
    }
    
    public IActionResult Watch(string id)
    {
        
        if(id == "404")
        {
            return View("404");
        }
        
        _logger.Log($"User has accessed {nameof(Watch)}");
        return View();
    }


}