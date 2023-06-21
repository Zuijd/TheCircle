namespace Portal.Controllers
{
    [TLSAccess]
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
                var logs = await _loggerService.GetAll();

                await _loggerService.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Log page!");

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
                    var serverResponse = await _loggerService.GetAllFromUsername(username, digSig, certificate);

                    //retrieve public key from certificate
                    var publicKey = _certificateService.GetPublicKeyOutOfCertificate(serverResponse.Certificate);

                    //verify digital signature
                    var isValid = _certificateService.VerifyDigSig(serverResponse.Message!, serverResponse.Signature!, publicKey);

                    //verification is succesful ? perform action : throw corresponding error
                    Console.WriteLine(isValid ? "LOG - SERVER PACKET IS VALID" : "LOG - SERVER PACKET IS INVALID");

                    await _loggerService.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has filtered Log page!");

                    if (isValid)
                    {
                        return View(serverResponse.Message);
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
