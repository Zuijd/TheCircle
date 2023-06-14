using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Services
{
    public interface IStreamService
    {
        public void AddStream(Streams stream);
        public void AddLiveMoment(Live live);
        public void AddBreakMoment(Break pauze);
        public void AddSatoshi(decimal satoshi, int streamId);
              
    }
}
