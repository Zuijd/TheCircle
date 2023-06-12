using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class StreamHub : Hub
{
    public async Task SendChunk(byte[] chunk)
    {
        await Clients.Others.SendAsync("ReceiveChunk", chunk);
    }
}
