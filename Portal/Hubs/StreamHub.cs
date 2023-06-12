using Microsoft.AspNetCore.SignalR;

namespace Portal.Hubs;

public class StreamHub : Hub
{
    public async Task SendStream(string message)
    {
        await Clients.All.SendAsync("ReceiveStream", message);
    }
}
