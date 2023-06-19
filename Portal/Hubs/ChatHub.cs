using Microsoft.AspNetCore.SignalR;

namespace Portal.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string username, string message, string streamUser)
        {
            Console.WriteLine("In method SendMessage " + streamUser);
            await Clients.Group(streamUser).SendAsync("ReceiveMessage", username, message, streamUser);
        }
        public async Task JoinGroup(string streamUserId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, streamUserId);
        }
    }
}