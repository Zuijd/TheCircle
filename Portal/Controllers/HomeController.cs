using System.Diagnostics;
using DomainServices.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using Microsoft.Extensions.Logging;
using DomainServices.Interfaces.Services;

namespace Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISatoshiCompensation _compensationService;
        private readonly IStreamRepository _streamRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IloggerService _customLogger;

        public HomeController(ISatoshiCompensation compensationService, IStreamRepository streamRepository, IHttpContextAccessor httpContextAccessor, IloggerService customLogger)
        {
            
            _compensationService = compensationService;
            _streamRepository = streamRepository;
            _httpContextAccessor = httpContextAccessor;
            _customLogger = customLogger;
        }

        public IActionResult Index()
        {
     
            _customLogger.Info("User has accessed the home page!");
            
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
}
