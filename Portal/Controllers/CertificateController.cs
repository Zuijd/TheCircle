﻿namespace Portal.Controllers
{
    [TLSAccess]
    public class CertificateController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly ICertificateService _certificateService;

        public CertificateController(ILogger<HomeController> logger, IUserService userService, ICertificateService certificateService)
        {
            _logger = logger;
            _userService = userService;
            _certificateService = certificateService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Test()
        {
            return View("../Home/Test");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TestPost(string message = "test")
        {
            ///// * CREATE DIGSIG FOR CREATEPOST (SERVICE) * /////
            //retrieve private key
            var privateKey = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "PrivateKey"));

            //retrieve certificate
            var certificate = ViewModelHelper.ConvertClaimToKey(await _userService.GetSpecificClaim(User.Identity?.Name!, "Certificate"));

            //create digital signature
            var digSig = _certificateService.CreateDigSig(message, privateKey);

            //call request to service
            var post = _certificateService.CreatePost(message, digSig, certificate);

            ///// * VERIFY REQUEST FROM CREATEPOST * /////
            //retrieve public key from certificate
            var publicKey = _certificateService.GetPublicKeyOutOfCertificate(post.Certificate);

            //verify digital signature
            var isValid = _certificateService.VerifyDigSig(message, digSig, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "SERVER PACKET IS VALID" : "SERVER PACKET IS INVALID");

            //everything has checked and return
            return RedirectToAction("Test", "Certificate");
        }
    }
}
