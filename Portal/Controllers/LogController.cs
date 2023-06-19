namespace Portal.Controllers
{
    public class LogController : Controller
    {

        private readonly ILoggerService _loggerService;
        private readonly ICertificateService _certificateService;

        public LogController(ILoggerService loggerService, ICertificateService certificateService)
        {
            _loggerService = loggerService;
            _certificateService = certificateService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logs() 
        {
            try
            {

                //TODO: voeg tls gezeik toe

                var logs = await _loggerService.GetAll();
                return View(logs);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logs(string username) 
        {
            try
            {

                Console.WriteLine(username);

                var logs = await _loggerService.GetAllFromUsername(username);
                Console.WriteLine(logs);
                
                return View(logs);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
