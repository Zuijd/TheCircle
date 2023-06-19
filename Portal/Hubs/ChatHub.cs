using Microsoft.AspNetCore.SignalR;

namespace Portal.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message, string username, string streamUser)
        {
            Console.WriteLine("Message in SendMessage!");
            Console.WriteLine("Message: ", message);
            Console.WriteLine("Username: ", username);
            Console.WriteLine("streamUser: ", streamUser);
            
            var httpContext = Context.GetHttpContext();
            var currentUser = httpContext!.User.Identity!.Name;

            await Clients.Group(currentUser).SendAsync("ReceiveMessage", message, username, streamUser);
        }
        public async Task JoinGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }
}