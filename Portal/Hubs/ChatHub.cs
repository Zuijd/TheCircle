using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string username, string message)
        {
            Console.WriteLine(username + " " + message);
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }
    }
}