using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class HouseDataPostDto
    {
        public string HouseName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int HouseSize { get; set; }
        public int RentPerMonth { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Road { get; set; } = null!;
        public List<FeatureLabelListDto>? FeaturLabelList { get; set; }
        public string? HouseImg { get; set; }
}

    public partial class HouseDataPutDto
    {
        public Guid HouseId { get; set; }
        public string HouseName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int HouseSize { get; set; }
        public int RentPerMonth { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Road { get; set; } = null!;
        public List<FeatureLabelListDto>? FeaturLabelList { get; set; }
        public string? HouseImg { get; set; }
    }

    public partial class HouseDataGetDto
    {
        public Guid HouseId { get; set; }

        public int UserId { get; set; }
        public string HouseName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int HouseSize { get; set; }
        public int RentPerMonth { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Road { get; set; } = null!;
        public List<FeatureLabelList> FeaturLabelList { get; set; }
        public string? HouseImg { get; set; }
    }
}
