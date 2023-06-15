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
        public void AddLiveMoment(dynamic live);
        public void AddBreakMoment(dynamic pauze);
        public void AddSatoshi(decimal satoshi, int streamId);
              
    }
}
