using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class UserData
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string? UserLocation { get; set; }
        public string? UserDescription { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserPassword { get; set; }
        public string? UserImgId { get; set; }
    }
}
