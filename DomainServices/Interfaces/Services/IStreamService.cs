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
        public int AddStream(dynamic streamInfo);
        public bool StopStream(dynamic streamInfo);
        public Task<bool> AddLiveMoment(dynamic live);
        public Task<bool> AddBreakMoment(dynamic pauze);
        public bool AddSatoshi(int streamId);
              
    }
}
