using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class StreamHub : Hub
{
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
}