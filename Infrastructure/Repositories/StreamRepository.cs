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

        public async Task<bool> addStream(Streams stream)
        {
            try
            {
                Console.WriteLine(stream.ToString());

                await _context.Streams.AddAsync(stream);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> saveBreakMoment(TimeSpan live, DateTime start, DateTime end, int Id)
        {
            var stream = await _context.Streams.FindAsync(Id);
            if (stream == null)
            {
                // Handle the case when the Streams entity with the given streamId doesn't exist
                return false;
            }

            var newBreak = new Break(live, start, end, Id);

            stream.BreakList ??= new List<Break>();
            stream.BreakList.Add(newBreak);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> saveLiveMoment(TimeSpan live, DateTime start, DateTime end, int Id)
        {
            var stream = await _context.Streams.FindAsync(Id);
            if (stream == null)
            { 
                // Handle the case when the Streams entity with the given streamId doesn't exist
                return false;
            }

            var newLive = new Live(live, start, end, Id);
            stream.LiveList ??= new List<Live>();
            stream.LiveList.Add(newLive);

            await _context.SaveChangesAsync();

            return true;
        }

        async Task<bool> IStreamRepository.SaveCompensation(decimal compensation, int Id)
        {
            var stream = await _context.Streams.FindAsync(Id);
            if (stream == null)
            {
                // Handle the case when the Streams entity with the given streamId doesn't exist
                return false;
            }

            stream.Satoshi = Decimal.Add(stream.Satoshi, compensation);
            await _context.SaveChangesAsync();

            return true;


        }
    }
}
