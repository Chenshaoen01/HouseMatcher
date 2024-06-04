using HouseMatcher.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseMatcher.Controllers
{
    public class UserController : Controller
    {
        public readonly HouseMatcherContext _HouseMatcherContext;
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IWebHostEnvironment _env;
        private int UserId;

        public UserController(HouseMatcherContext houseMatcherContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _HouseMatcherContext = houseMatcherContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            Claim UserIdString = httpContextAccessor.HttpContext.User.FindFirst(data => data.Type == "UserId");
            if(UserIdString != null)
            {
                UserId = Convert.ToInt32(UserIdString.Value);
            }
        }

        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult RegisterPage()
        {
            return View();
        }

        public IActionResult UserInfo()
        {
            UserData targetData = _HouseMatcherContext.UserData.Find(UserId);
            return View(targetData);
        }

        //登入
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserLoginDto LoginData)
        {
            var user = (from a in _HouseMatcherContext.UserData
                        where a.UserEmail == LoginData.UserEmail
                        && a.UserPassword == HashPassword(LoginData.UserPassword)
                        select a).SingleOrDefault();

            if (user == null)
            {
                return NotFound("帳號密碼錯誤");
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("UserId", user.UserId.ToString()),
                };

                if(user.UserImgId != null)
                {
                    claims.Add(new Claim("UserImg", user.UserImgId));
                } else
                {
                    claims.Add(new Claim("UserImg", ""));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties {
                    RedirectUri = "/Home"
                };

                await HttpContext.SignInAsync(
                      CookieAuthenticationDefaults.AuthenticationScheme,
                      new ClaimsPrincipal(claimsIdentity),
                      authProperties);

                return Ok("登入成功");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register([FromBody] UserRegisterDto value)
        {
            List<UserData> SameEmailUser = _HouseMatcherContext.UserData.Where(UserData => UserData.UserEmail == value.UserEmail).ToList();

            if(SameEmailUser.Count > 0)
            {
                return BadRequest("此信箱已被註冊");
            } else
            {
                string hashedPassword = HashPassword(value.UserPassword);

                UserData postUserData = new UserData()
                {
                    UserEmail = value.UserEmail,
                    UserName = value.UserName,
                    UserPassword = hashedPassword
                };
                _HouseMatcherContext.UserData.Add(postUserData);
                _HouseMatcherContext.SaveChanges();
                return Ok("註冊成功");
            }
        }

        public string HashPassword(string UnhashedPassword)
        {
            byte[] salt = Encoding.ASCII.GetBytes(_configuration["HashSalt"]); // divide by 8 to convert bits to bytes
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: UnhashedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8)
            );

            return hashedPassword;
        }

        [AllowAnonymous]
        public async Task Logout()
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        //修改使用者資料
        [HttpPut]
        public async Task<ActionResult> UserPut([FromBody] UserDataPutDto value)
        {
            List<UserData> SameEmailUser = _HouseMatcherContext.UserData.Where(UserData => UserData.UserEmail == value.UserEmail).ToList();
            UserData targetUser = _HouseMatcherContext.UserData.Where(user => user.UserId == UserId).FirstOrDefault();

            if (SameEmailUser.Count > 0 && targetUser.UserEmail != value.UserEmail)
            {
                return BadRequest("此信箱已被註冊");
            } else
            {
                if(targetUser.UserImgId != value.UserImgId && targetUser.UserImgId != null && targetUser.UserImgId != "")
                {
                    string filePath = _env.ContentRootPath + @"\wwwroot\" + targetUser.UserImgId;
                    System.IO.File.Delete(filePath);
                }

                targetUser.UserName = value.UserName;
                targetUser.UserEmail = value.UserEmail;
                targetUser.UserLocation = value.UserLocation;
                targetUser.UserDescription = value.UserDescription;
                targetUser.UserPhoneNumber = value.UserPhoneNumber;
                targetUser.UserImgId = value.UserImgId;

                _HouseMatcherContext.SaveChanges();

                // 更新聲明
                var identity = (ClaimsIdentity)User.Identity;
                identity.RemoveClaim(identity.FindFirst(ClaimTypes.Email));
                identity.RemoveClaim(identity.FindFirst(ClaimTypes.Name));
                identity.RemoveClaim(identity.FindFirst("UserId"));
                identity.RemoveClaim(identity.FindFirst("UserImg"));

                identity.AddClaim(new Claim(ClaimTypes.Email, value.UserEmail));
                identity.AddClaim(new Claim(ClaimTypes.Name, value.UserName));
                identity.AddClaim(new Claim("UserId", targetUser.UserId.ToString()));

                if (value.UserImgId != null)
                {
                    identity.AddClaim(new Claim("UserImg", value.UserImgId));
                }
                else
                {
                    identity.AddClaim(new Claim("UserImg", ""));
                }

                // 重新登入使用者，以應用新的聲明
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = true });

                return Ok("資料更新成功");
            }
        }
    }
}
