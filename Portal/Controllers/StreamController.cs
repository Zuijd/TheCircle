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

    public IActionResult Watch()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Message([FromBody] ChatViewModel chatViewModel)
    {
        Debug.WriteLine("eee");
        try
        {
            Debug.WriteLine(chatViewModel.Username);

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Hier ben ik");
                Debug.WriteLine(chatViewModel.Username);
                Debug.WriteLine(chatViewModel.Message);
            }
        }
        catch (Exception e)
        {
            ModelState.AddModelError(e.Message, e.Message);
        }

        return PartialView("_Chat");
    }
}