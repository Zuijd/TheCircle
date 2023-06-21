using Domain;
using DomainServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class MessageService : IMessageService
    {

        private readonly IMessageRepository _messageRepository;
        private readonly ICertificateService _certificateService;

        public MessageService(IMessageRepository messageRepository, ICertificateService certificateService)
        {
            _messageRepository = messageRepository;
            _certificateService = certificateService;
        }
        public async Task<PKC> CreateMessage(object content, byte[] signature, byte[] certificate)
        {
            var publicKey = _certificateService.GetPublicKeyOutOfCertificate(certificate);

            //verify digital signature
            var isValid = _certificateService.VerifyDigSig(content, signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "MESSAGE - CLIENT PACKET IS VALID" : "MESSAGE - CLIENT PACKET IS INVALID");

            var createMessage = await _messageRepository.CreateMessage((Message)content);

            return new PKC()
            {
                Message = createMessage,
                Signature = _certificateService.CreateDigSig(createMessage, _certificateService.GetPrivateKeyFromServer()),
                Certificate = _certificateService.GetCertificateFromServer(),
            };
        }
    }
}
