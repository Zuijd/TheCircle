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
        public Task<bool> addStream(Streams stream);
        public Task<bool> saveLiveMoment(TimeSpan live, DateTime start, DateTime end, int streamid);
        public Task<bool> saveBreakMoment(TimeSpan live, DateTime start, DateTime end, int streamid);
        public Task<bool> SaveCompensation(decimal compensation, int streamId);
    }
}

