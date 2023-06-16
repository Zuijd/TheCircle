using System.Diagnostics;
using DomainServices.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using Microsoft.Extensions.Logging;
using DomainServices.Interfaces.Services;

namespace Portal.Controllers
{
    [TLSAccess]
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerService _logger;

        public HomeController(IHttpContextAccessor httpContextAccessor, ILoggerService logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.Log("User has accessed the home page!");
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
}