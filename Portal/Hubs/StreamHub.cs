using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class StreamHub : Hub
{
    public async Task SendChunk(string streamId, string chunk)
    {
        await Clients.Others.SendAsync("ReceiveChunk", streamId, chunk);
    }
}
