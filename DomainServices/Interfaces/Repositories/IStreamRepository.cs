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
        public Task<bool> saveLiveMoment(Live live);
        public Task<bool> saveBreakMoment(Break pauze);
        public Task<bool> SaveCompensation(decimal compensation, int streamId);
    }
}

