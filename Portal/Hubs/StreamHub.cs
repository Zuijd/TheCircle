using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Portal.Hubs
{
    public class StreamHub : Hub
    {
        public int watcherCount = 0;

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
}