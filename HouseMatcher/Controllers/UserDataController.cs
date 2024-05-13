using HouseMatcher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        public readonly HouseMatcherContext _HouseMatcherContext;
        public readonly IConfiguration _configuration;
        public UserDataController(HouseMatcherContext houseMatcherContext, IConfiguration configuration)
        {
            _HouseMatcherContext = houseMatcherContext;
            _configuration = configuration;
        }

        // 取得個別使用者資料
        [HttpGet("{id}")]
        public ActionResult<UserData> Get(int id)
        {
            UserData targetData = _HouseMatcherContext.UserData.Find(id);
            if(targetData == null)
            {
                return NotFound("無此筆資料");
            }

            return targetData;
        }

        //修改使用者資料
        [HttpPut]
        public ActionResult Put([FromBody] UserDataPutDto value)
        {
            //UserData targetUser = _HouseMatcherContext.UserData.Where(user => user.UserId == value.UserId).FirstOrDefault();

            //targetUser.UserName = value.UserName;
            //targetUser.UserEmail = value.UserEmail;
            //targetUser.UserLocation = value.UserLocation;
            //targetUser.UserDescription = value.UserDescription;
            //targetUser.UserPhoneNumber = value.UserPhoneNumber;
            //targetUser.UserImgId = value.UserImgId;

            //_HouseMatcherContext.SaveChanges();
            return Ok("更新成功");
        }

        //刪除使用者
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var deleteTarget = _HouseMatcherContext.UserData.Find(id);
            if(deleteTarget == null)
            {
                return NotFound("無此資料");
            }
            else
            {
                _HouseMatcherContext.UserData.RemoveRange(deleteTarget);
                _HouseMatcherContext.SaveChanges();
                return Ok("刪除成功");
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
    }
}
