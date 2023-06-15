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
            var streamId = HttpContext.Session.GetInt32("StreamId");
            var endStream = DateTime.Parse(streamInfo.GetProperty("endStream").GetString());
            var durationStream = streamInfo.GetProperty("durationStream").GetInt32();
            var succes = this._streamRepository.StopStream((int)streamId!, endStream, durationStream);
            HttpContext.Session.Remove("StreamId");
            return succes;
        }

        public void AddBreakMoment(dynamic pauzeInfo)
        {
            this._streamRepository.saveBreakMoment(pauzeInfo);
        }

        public void AddLiveMoment(dynamic liveInfo)
        {
            this._streamRepository.saveLiveMoment(liveInfo);
        }

        public void AddSatoshi(decimal satoshi, int streamId)
        {
            this._streamRepository.SaveCompensation(satoshi, streamId);
        }

       
    }
}
