using System.Diagnostics;
using DomainServices.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISatoshiCompensation _compensationService;
    private readonly IStreamRepository _streamRepository;

    public HomeController(ILogger<HomeController> logger, ISatoshiCompensation compensationService, IStreamRepository streamRepository)
    {
        _logger = logger;
        _compensationService = compensationService;
        _streamRepository = streamRepository;
    }

    public IActionResult Index()
    {
        // Test for logging and Satoshi compensation --> Uncomment below to run
        _logger.LogInformation("Executing MyAction");
        var compensation = _compensationService.CalculateCompensation(new TimeSpan(6, 20, 10));
        //_streamRepository.SaveCompensation(compensation, 1);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
