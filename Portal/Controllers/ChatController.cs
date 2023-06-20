namespace Portal.Controllers
{
    [TLSAccess]
    public class ChatController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly ICertificateService _certificateService;
        private readonly ILoggerService _logger;

        public ChatController(IMessageService messageService, IUserService userService, ICertificateService certificateService, ILoggerService logger)
        {
            _messageService = messageService;
            _userService = userService;
            _certificateService = certificateService;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has accessed Chat page!");
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Message([FromBody] ChatViewModel chatViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    Message message = new()
                    {
                        User = await _userService.GetUserByName(User.Identity?.Name!),
                        MessageBody = chatViewModel.Message!
                    };

                    ///// * CREATE DIGSIG FOR CREATEPOST (SERVICE) * /////
                    //retrieve private key
                    var privateKey = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "PrivateKey"));

                    //retrieve certificate
                    var certificate = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "Certificate"));

                    //create digital signature
                    var digSig = _certificateService.CreateDigSig(message, privateKey);

                    //call request to service
                    var serverResponse = await _messageService.CreateMessage(message, digSig, certificate);

                    ///// * VERIFY REQUEST FROM CREATEPOST * /////
                    //retrieve public key from certificate
                    var publicKey = _certificateService.GetPublicKeyOutOfCertificate(serverResponse.Certificate);

                    //verify digital signature
                    var isValid = _certificateService.VerifyDigSig(serverResponse.Message, serverResponse.Signature, publicKey);

                    //verification is succesful ? perform action : throw corresponding error
                    Console.WriteLine(isValid ? "SERVER PACKET IS VALID" : "SERVER PACKET IS INVALID");

                    _logger.Log(User.Identity!.Name!, $"{User.Identity!.Name!} has posted a message!");
                }
            }
            catch (Exception e)
            {

                ModelState.AddModelError(e.Message, e.Message);
            }

            //Console.WriteLine(chatViewModel.ViewName);

            if (chatViewModel.ViewName == "Stream Index")
            {
                return RedirectToAction("Index", "Stream");
            }
            else if (chatViewModel.ViewName == "Watch")
            {
                return RedirectToAction("Watch", "Stream");
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
