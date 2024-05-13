using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string UserId;
        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            UserId = _httpContextAccessor.HttpContext.User.FindFirst(data => data.Type == "UserId").Value;
        }

        public override async Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, "Room"+ UserId);
        }

        public async Task SendMessage(string TargetId, string Message)
        {
            await Clients.Group("Room"+ TargetId).SendAsync("ReceiveMessage", UserId, Message);
        }
    }
}
