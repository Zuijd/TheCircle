using Domain;
using DomainServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class MessageService : IMessageService
    {

        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<bool> CreateMessage(Message message)
        {
            return await _messageRepository.CreateMessage(message);
        }
    }
}
