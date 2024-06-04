namespace HouseMatcher.Models
{
    public partial class MessageData
    {
        public int MessageId { get; set; }
        public string MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid HouseDataId { get; set; }
    }

    public partial class MessageDataGet
    {
        public int MessageId { get; set; }
        public string MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public string SenderName { get; set; } = null!;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
        public Guid HouseDataId { get; set; }
        public string HouseName { get; set; } = null!;
        public bool IsMyHouse { get; set; }
    }
}
