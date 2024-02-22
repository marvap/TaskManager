using Microsoft.AspNetCore.SignalR;
using TaskManager.Data;
using TaskManager.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager
{
    public class ChatHub : Hub
    {
        public const string HubUrl = "/chat";

        private DataMediator _dataMediator = new DataMediator();


        public async Task Broadcast(int taskId, int creatorUserId, string message)
        {
            DateTime timeCreated = DateTime.Now;
            await _dataMediator.AddChatMessageAsync(taskId, creatorUserId, message, timeCreated);

            User user = await _dataMediator.GetUserByIdAsync(creatorUserId);
            string creatorName = user?.Name;

            await Clients.All.SendAsync("Broadcast", taskId, creatorUserId, $"{creatorName} {timeCreated}", message);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }
    }
}
