using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainServices.Interfaces.Repositories;
using Infrastructure.Contexts;
using Domain;
using System.IO;

namespace Infrastructure.Repositories
{
    public class StreamRepository : IStreamRepository
    {
        private readonly ApplicationDbContext _context;

        public StreamRepository(ApplicationDbContext context) => _context = context;

        public async Task<int> addStream(Streams stream)
        {
            try
            {
                await _context.Streams.AddAsync(stream);
                await _context.SaveChangesAsync();
                return stream.Id;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> StopStream(int streamId, DateTime endStream, TimeSpan durationStream, decimal satoshi)
        {
            var stream = await _context.Streams.FindAsync(streamId);
            if (stream == null)
            {
                return false;
            }

            stream.IsLive = false;
            stream.End = endStream;
            stream.Satoshi = satoshi;
            stream.Duration = durationStream;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> saveBreakMoment(Break newBreak, int streamId)
        {
            var stream = await _context.Streams.FindAsync(streamId);
            if (stream == null)
            {
                // Handle the case when the Streams entity with the given streamId doesn't exist
                return false;
            }

            stream.BreakList ??= new List<Break>();
            stream.BreakList.Add(newBreak);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> saveLiveMoment(Live live, int streamId)
        {
            var stream = await _context.Streams.FindAsync(streamId);
            if (stream == null)
            {
                // Handle the case when the Streams entity with the given streamId doesn't exist
                return false;
            }

            stream.LiveList ??= new List<Live>();
            stream.LiveList.Add(live);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Live>> GetLiveMoments(int StreamId)
        {
            var stream = await _context.Streams
                .Include(s => s.LiveList) // Explicitly include the LiveList
                .FirstOrDefaultAsync(s => s.Id == StreamId);

            if (stream == null)
            {
                // Handle the case when the Streams entity with the given streamId doesn't exist
                return null;
            }

            var LiveList = stream.LiveList;
            return LiveList;
        }

        public async Task<List<Streams>> GetStreams(string username)
        {
            var streams = await _context.Streams
                .Include(u => u.BreakList)
                .Include(u => u.LiveList)
                .Where(u => u.UserName == username)
                .OrderByDescending(u => u.Id)
                .ToListAsync();

            if (streams == null)
            {
                return new List<Streams>();
            }

            return streams;
        }

    }
}
