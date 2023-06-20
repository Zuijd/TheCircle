using Microsoft.AspNetCore.SignalR;

namespace Portal.Hubs
{
    public class StreamHub : Hub
    {
        public async Task SendChunk(byte[] chunk)
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext!.User.Identity!.Name;

            await Clients.Group(userName).SendAsync("ReceiveChunk", chunk);
        }

        public async Task JoinGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }
}