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
        public Task<int> AddStream(dynamic streamInfo);
        public Task<bool> StopStream(dynamic streamInfo);
        public Task<bool> AddLiveMoment(dynamic live);
        public Task<bool> AddBreakMoment(dynamic pauze);
        public Task<decimal> GetSatoshi(int streamId);


    }
}
