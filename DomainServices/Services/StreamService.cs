using Domain;
using DomainServices.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace DomainServices.Services
{
    public class StreamService : IStreamService
    {
        private readonly IStreamRepository _streamRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public StreamService(IStreamRepository streamRepository, IHttpContextAccessor httpContextAccessor)
        {
            _streamRepository = streamRepository;
            _contextAccessor = httpContextAccessor;
        }

        public int AddStream(dynamic newStreamInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var username = HttpContext.Session.GetString("Username");
            var startStream = DateTime.Parse(newStreamInfo.GetProperty("startStream").GetString());
            var newStream = new Streams(username, true, startStream);
            var streamId = this._streamRepository.addStream(newStream).GetAwaiter().GetResult();
            HttpContext.Session.SetInt32("StreamId",streamId);
            return streamId;
        }
        public bool StopStream(dynamic streamInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var streamId = (int) HttpContext.Session.GetInt32("StreamId")!;
            var endStream = DateTime.Parse(streamInfo.GetProperty("endStream").GetString());
            var durationStream = streamInfo.GetProperty("durationStream").GetInt32();
            var succes = this._streamRepository.StopStream(streamId, endStream, durationStream);
            //Add satoshi call + WHATSUPPP!!!!
            HttpContext.Session.Remove("StreamId");
            return succes;
        }

        public Task<bool> AddBreakMoment(dynamic pauzeInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var streamId = (int)HttpContext.Session.GetInt32("StreamId")!;
            var startBreak = DateTime.Parse(pauzeInfo.GetProperty("startBreak").GetString());
            var endBreak = DateTime.Parse(pauzeInfo.GetProperty("endBreak").GetString());
            TimeSpan duration = endBreak - startBreak;
            var newBreak = new Break(startBreak, endBreak, duration);
            return this._streamRepository.saveBreakMoment(newBreak, streamId);
        }

        public Task<bool> AddLiveMoment(dynamic liveInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var streamId = (int)HttpContext.Session.GetInt32("StreamId")!;
            var startLive = DateTime.Parse(liveInfo.GetProperty("startLive").GetString());
            var endLive = DateTime.Parse(liveInfo.GetProperty("endLive").GetString());
            TimeSpan duration = endLive - startLive;
            var newLive = new Live(startLive, endLive, duration);
            return this._streamRepository.saveLiveMoment(newLive,streamId);
        }


        bool IStreamService.AddSatoshi(int streamId)
        {
            throw new NotImplementedException();
        }
    }
}
