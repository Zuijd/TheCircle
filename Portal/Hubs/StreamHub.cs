using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Portal.Hubs
{
    public class StreamHub : Hub
    {
        public async Task SendChunk(byte[] chunk)
        {
            await Clients.Others.SendAsync("ReceiveChunk", chunk);
        }
    }
}