using DomainServices.Interfaces.Repositories;

namespace DomainServices.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerRepository _loggerRepository;
        private readonly ICertificateService _certificateService;
        private string _user;

        public LoggerService(ILoggerRepository loggerRepository, ICertificateService certificateService)
        {
            _loggerRepository = loggerRepository;
            _certificateService = certificateService;
        }

        public async Task Log(string user, string message)
        {
            await _loggerRepository.addLog(new Log(user, message));
        }
        
        public async Task<List<Log>> GetAll() {
            return await _loggerRepository.GetAll();
        }

        public async Task<PKC> GetAllFromUsername(string username, byte[] signature, byte[] certificate)
        {
            var publicKey = _certificateService.GetPublicKeyOutOfCertificate(certificate);

            //verify digital signature
            var isValid = _certificateService.VerifyDigSig(username, signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "CLIENT PACKET IS VALID" : "CLIENT PACKET IS INVALID");

            var content = await _loggerRepository.GetAllFromUsername(username);

            return new PKC()
            {
                Message = content,
                Signature = _certificateService.CreateDigSig(content, _certificateService.GetPrivateKeyFromServer()),
                Certificate = _certificateService.GetCertificateFromServer(),
            };
        }
    }
}
