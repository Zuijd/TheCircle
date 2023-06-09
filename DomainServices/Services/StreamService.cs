﻿using Domain;
using DomainServices.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using DomainServices.Interfaces.Services;
using System.Reflection.Metadata;


namespace DomainServices.Services
{
    public class StreamService : IStreamService
    {
        private readonly IStreamRepository _streamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateService _certificateService;

        public StreamService(IStreamRepository streamRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ICertificateService certificateService)
        {
            _streamRepository = streamRepository;
            _contextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _certificateService = certificateService;
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
            var satoshi = streamInfo.GetProperty("earnedSatoshi").GetString();
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(durationStream);
            var succes = await this._streamRepository.StopStream(streamId, endStream, timeSpan, satoshi);
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
        public async Task<decimal> GetSatoshi(int streamId)
        {
            var liveList = await this._streamRepository.GetLiveMoments(streamId);
            if (liveList == null) return 0;
            decimal satoshi = 0.00000000M;   
            foreach (var live in liveList) {
                satoshi = satoshi + CalculateSatoshi(live.Duration);
                 }
            return satoshi;
        }

        private const decimal InitialCompensation = 0.00000001M;
        private const double CompensationMultiplier = 2;

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
        public async Task<List<Streams>> GetStreams()
        {
            var HttpContext = _contextAccessor.HttpContext;
            var username = HttpContext.Session.GetString("Username");
            return await _streamRepository.GetStreams(username);
        }


        public PKC ValidateChunk(byte[] chunk, byte[] signature, byte[] certificate)
        {
            var publicKey = _certificateService.GetPublicKeyOutOfCertificate(certificate);

            //verify digital signature
            var isValid = _certificateService.VerifyDigSig(chunk, signature, publicKey);

            //verification is succesful ? perform action : throw corresponding error
            Console.WriteLine(isValid ? "STREAM - CLIENT PACKET IS VALID" : "STREAM - CLIENT PACKET IS INVALID");

            return new PKC()
            {
                Message = chunk,
                Signature = _certificateService.CreateDigSig(chunk, _certificateService.GetPrivateKeyFromServer()),
                Certificate = _certificateService.GetCertificateFromServer(),
            };
        }
        
        public async Task<bool> SaveChunk(byte[] chunk)
        {
            DateTime timeStamp = DateTime.Now;

            return await _streamRepository.SaveChunk(timeStamp, chunk);
        }
    }
}
