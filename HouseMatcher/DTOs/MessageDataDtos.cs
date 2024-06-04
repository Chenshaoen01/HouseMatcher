using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class MessageDataPostDto
    {
        public string MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public Guid HouseDataId { get; set; }
    }

    public partial class MessageDataGetDto
    {
        public string MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public Guid HouseDataId { get; set; }
        public string HouseDataName { get; set; }
    }
}
