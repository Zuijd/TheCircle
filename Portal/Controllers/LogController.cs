using Portal.Models;

namespace Portal.Controllers
{
    public class LogController : Controller
    {

        private readonly ILoggerService _loggerService;
        private readonly ICertificateService _certificateService;
        private readonly IUserService _userService;

        public LogController(ILoggerService loggerService, ICertificateService certificateService, IUserService userService)
        {
            _loggerService = loggerService;
            _certificateService = certificateService;
            _userService = userService;
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
                if (username == null)
                {
                    var logs = await _loggerService.GetAll();
                    return View(logs);
                }
                else
                {
                    //retrieve private key
                    var privateKey = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "PrivateKey"));

                    //retrieve certificate
                    var certificate = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "Certificate"));

                    //create digital signature
                    var digSig = _certificateService.CreateDigSig(username, privateKey);

                    //call request to service
                    var serverResponse = _certificateService.CreatePost(username, digSig, certificate);

                    //retrieve public key from certificate
                    var publicKey = _certificateService.GetPublicKeyOutOfCertificate(serverResponse.Certificate);

                    //verify digital signature
                    var isValid = _certificateService.VerifyDigSig(serverResponse.Message!, serverResponse.Signature!, publicKey);

                    if (isValid)
                    {
                        var logs = await _loggerService.GetAllFromUsername(username);
                        return View(logs);
                    }
                    else
                    {
                        return View(new List<Log>());
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
