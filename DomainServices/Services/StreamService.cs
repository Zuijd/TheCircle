﻿using Domain;
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

        public async Task<int> AddStream(dynamic newStreamInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var username = HttpContext.Session.GetString("Username");
            var startStream = DateTime.Parse(newStreamInfo.GetProperty("startStream").GetString());
            var newStream = new Streams(username, true, startStream);
            var streamId = await this._streamRepository.addStream(newStream);
            HttpContext.Session.SetInt32("StreamId",streamId);
            return streamId;
        }
        public async Task<bool> StopStream(dynamic streamInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var streamId = (int) HttpContext.Session.GetInt32("StreamId")!;
            var endStream = DateTime.Parse(streamInfo.GetProperty("endStream").GetString());
            var durationStream = streamInfo.GetProperty("durationStream").GetInt32();
            var succes = await this._streamRepository.StopStream(streamId, endStream, durationStream);
            await this.AddSatoshi(streamId);
            HttpContext.Session.Remove("StreamId");
            return succes;
        }

        public async Task<bool> AddBreakMoment(dynamic pauzeInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var streamId = (int)HttpContext.Session.GetInt32("StreamId")!;
            var startBreak = DateTime.Parse(pauzeInfo.GetProperty("startBreak").GetString());
            var endBreak = DateTime.Parse(pauzeInfo.GetProperty("endBreak").GetString());
            TimeSpan duration = endBreak - startBreak;
            var newBreak = new Break(startBreak, endBreak, duration);
            return await this._streamRepository.saveBreakMoment(newBreak, streamId);
        }

        public async Task<bool> AddLiveMoment(dynamic liveInfo)
        {
            var HttpContext = _contextAccessor.HttpContext;
            var streamId = (int)HttpContext.Session.GetInt32("StreamId")!;
            var startLive = DateTime.Parse(liveInfo.GetProperty("startLive").GetString());
            var endLive = DateTime.Parse(liveInfo.GetProperty("endLive").GetString());
            TimeSpan duration = endLive - startLive;
            var newLive = new Live(startLive, endLive, duration);
            return await this._streamRepository.saveLiveMoment(newLive,streamId);
        }

        // Satoshi
        public async Task<bool> AddSatoshi(int streamId)
        {
            var liveList = await this._streamRepository.GetLiveMoments(streamId);
           
            foreach (var live in liveList) {
                satoshi = satoshi + CalculateSatoshi(live.Duration);
                 }

            return await this._streamRepository.SaveCompensation(satoshi, streamId);
        }

        private const decimal InitialCompensation = 0.00000001M;
        private const double CompensationMultiplier = 2;
        public decimal satoshi;

        public decimal CalculateSatoshi(TimeSpan streamDuration)
        {
            //Calculate the compensation based on the duration

            //number of consecutive hours
            int consecutiveHours = (int)streamDuration.TotalHours;

            // Calculate the compensation

            decimal compensation = InitialCompensation * (decimal)Math.Pow(CompensationMultiplier, consecutiveHours - 1);

            //_logger.Info($"calculated {compensation} Satoshi from {consecutiveHours} consecutive hours streamed ");

            return compensation;

        }
    }
}
