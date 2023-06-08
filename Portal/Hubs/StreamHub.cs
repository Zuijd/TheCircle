using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace Portal.Hubs;

public class StreamHub : Hub
{
    public class SignalingHub : Hub
    {
        public async Task SendOffer(string offer)
        {
            await Clients.Others.SendAsync("OfferReceived", offer);
        }

        public async Task SendAnswer(string answer)
        {
            await Clients.Others.SendAsync("AnswerReceived", answer);
        }

        public async Task SendIceCandidate(string candidate)
        {
            await Clients.Others.SendAsync("IceCandidateReceived", candidate);
        }
    }
}

