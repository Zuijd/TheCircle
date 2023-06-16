using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string username, string message, string streamUserId)
        {
            Console.WriteLine("In method SendMessage " + streamUserId);
            await Clients.Group(streamUserId).SendAsync("ReceiveMessage", username, message, streamUserId);
        }
        public async Task JoinGroup(string streamUserId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, streamUserId);
        }
    }
}