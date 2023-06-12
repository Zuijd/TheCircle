using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class StreamHub : Hub
{
    static int watcherCount = 0;

    public async Task SendChunk(byte[] chunk)
    {
        await Clients.Others.SendAsync("ReceiveChunk", chunk);
    }

    public async Task SendIceCandidate(object iceCandidate)
    {
        await Clients.Others.SendAsync("IceCandidateReceived", iceCandidate);
    }

    public async Task SendOffer(object offer)
    {
        await Clients.Others.SendAsync("OfferReceived", offer);
    }

    public async Task SendAnswer(object answer)
    {
        await Clients.Others.SendAsync("AnswerReceived", answer);
    }
    public override async Task OnConnectedAsync()
    {
        watcherCount++;
        await Clients.All.SendAsync("UpdateWatcherCount", watcherCount);

        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        watcherCount--;
        await Clients.All.SendAsync("UpdateWatcherCount", watcherCount);

        await base.OnDisconnectedAsync(exception);
    }
}