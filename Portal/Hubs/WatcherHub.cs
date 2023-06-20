using Domain;
using Microsoft.AspNetCore.SignalR;

public class UserHandler
{
    public static HashSet<string> ConnectedIds = new HashSet<string>();
}

public class WatcherHub : Hub
{
    public override Task OnConnectedAsync()
    {
        UserHandler.ConnectedIds.Add(Context.ConnectionId);
        // -1 because of streamer himself
        Clients.All.SendAsync("UpdateWatcherCount", UserHandler.ConnectedIds.Count - 1);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        UserHandler.ConnectedIds.Remove(Context.ConnectionId);
        // -1 because of streamer himself
        Clients.All.SendAsync("UpdateWatcherCount", UserHandler.ConnectedIds.Count - 1);
        return base.OnDisconnectedAsync(exception);
    }
}