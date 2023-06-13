using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

public class StreamController : Controller
{
    private readonly ILogger<StreamController> _logger;

    public StreamController(ILogger<StreamController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

}