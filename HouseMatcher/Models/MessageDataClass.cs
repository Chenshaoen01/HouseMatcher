namespace HouseMatcher.Models
{
    public partial class MessageData
    {
        public int MessageId { get; set; }
        public string MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int HouseDataId { get; set; }
    }
}
