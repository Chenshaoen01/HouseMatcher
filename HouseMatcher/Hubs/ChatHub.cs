using HouseMatcher.Models;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly HouseMatcherContext _HouseMatcherContext;
        private string UserId;
        public ChatHub(HouseMatcherContext houseMatcherContext, IHttpContextAccessor httpContextAccessor)
        {
            _HouseMatcherContext = houseMatcherContext;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(data => data.Type == "UserId").Value;
        }

        public override async Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, "Room" + UserId);
        }

        public async Task SendMessage(string Message, string TargetReceiverId, string TargetHouseId, string TargetHouseName, string TargetReceiverName, string TargetSenderName)
        {
            MessageData postMessageData = new MessageData()
            {
                MessageDescription = Message,
                SenderId = Int32.Parse(UserId),
                ReceiverId = Int32.Parse(TargetReceiverId),
                HouseDataId = new Guid(TargetHouseId),
                CreatedTime = DateTime.Now
            };

            MessageDataGet messageDataGetForReceiver = new MessageDataGet()
            {
                MessageDescription = Message,
                SenderId = Int32.Parse(UserId),
                SenderName = TargetSenderName,
                ReceiverId = Int32.Parse(TargetReceiverId),
                ReceiverName = TargetReceiverName,
                CreatedTime = postMessageData.CreatedTime,
                HouseDataId = new Guid(TargetHouseId),
                HouseName = TargetHouseName,
                IsMyHouse = false
            };

            MessageDataGet messageDataGetForSender = messageDataGetForReceiver;
            messageDataGetForSender.IsMyHouse = true;

            _HouseMatcherContext.MessageData.Add(postMessageData);
            _HouseMatcherContext.SaveChanges();
            await Clients.Group("Room" + TargetReceiverId).SendAsync("ReceiveMessage", messageDataGetForReceiver);
            await Clients.Group("Room" + UserId).SendAsync("ReceiveMessage", messageDataGetForSender);
        }
    }
}