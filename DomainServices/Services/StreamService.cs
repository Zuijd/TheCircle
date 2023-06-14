using Domain;
using DomainServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class StreamService : IStreamService
    {
        private readonly IStreamRepository _streamRepository;

        public StreamService(IStreamRepository streamRepository)
        {
            _streamRepository = streamRepository;
        }

        public void AddBreakMoment(Break pauze)
        {
            this._streamRepository.saveBreakMoment(pauze);
        }

        public void AddLiveMoment(Live live)
        {
            this._streamRepository.saveLiveMoment(live);
        }

        public void AddSatoshi(decimal satoshi, int streamId)
        {
            this._streamRepository.SaveCompensation(satoshi, streamId);
        }

        public void AddStream(Streams stream)
        {
            this._streamRepository.addStream(stream);
        }
    }
}
