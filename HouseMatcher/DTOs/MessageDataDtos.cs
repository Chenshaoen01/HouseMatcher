using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class MessageDataPostDto
    {
        public string? MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int HouseDataId { get; set; }
    }

    public partial class MessageDataGetDto
    {
        public string? MessageDescription { get; set; } = null!;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int HouseDataId { get; set; }
        public int HouseDataName { get; set; }
    }
}
