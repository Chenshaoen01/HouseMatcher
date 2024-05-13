using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class HouseData
    {
        public Guid HouseId { get; set; }
        public int UserId { get; set; }
        public string HouseName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; }
        public int HouseSize { get; set; }
        public int RentPerMonth { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Road { get; set; } = null!;
        public DateTime UpdateTime { get; set; }
        public string? HouseImg { get; set; }
    }
}
