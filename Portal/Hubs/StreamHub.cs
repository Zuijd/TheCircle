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

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnectedAsync called");
            UserHandler.ConnectedIds.Add(Context.ConnectionId);

            //Deze volgende line roept de hele methode opnieuw aan waardoor de watchCount +2 wordt gedaan ipv +1
            Clients.All.SendAsync("UpdateWatcherCount", UserHandler.ConnectedIds.Count);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            Clients.All.SendAsync("UpdateWatcherCount", UserHandler.ConnectedIds.Count);
            return base.OnDisconnectedAsync(exception);
        }
    }
}