using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Repositories
{
    public interface IStreamRepository
    {
        public Task<int> addStream(Streams stream);
        public Task<bool> StopStream(int streamId, DateTime endStream, int durationStream);
        public Task<bool> saveLiveMoment(Live live,int streamId);
        public Task<bool> saveBreakMoment(Break pauze, int streamId);
        public Task<bool> SaveCompensation();
    }
}

