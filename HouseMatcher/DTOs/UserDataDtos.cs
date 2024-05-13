using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class UserDataPostDto
    {
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string? UserLocation { get; set; }
        public string? UserDescription { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserPassword { get; set; }
    }

    public partial class UserDataPutDto
    {
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string? UserLocation { get; set; }
        public string? UserDescription { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserImgId { get; set; }
    }

    public partial class UserLoginDto
    {
        public string UserEmail { get; set; } = null!;

        public string UserPassword { get; set; } = null!;
    }

    public partial class UserRegisterDto
    {
        public string UserName { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public string UserPassword { get; set; } = null!;
    }
}
