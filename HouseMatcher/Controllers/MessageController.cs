using HouseMatcher.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseMatcher.Controllers
{
    public class MessageController : Controller
    {
        public readonly HouseMatcherContext _HouseMatcherContext;
        public readonly IConfiguration _configuration;
        private string UserId;
        public MessageController(HouseMatcherContext houseMatcherContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _HouseMatcherContext = houseMatcherContext;
            _configuration = configuration;
            UserId = httpContextAccessor.HttpContext.User.FindFirst(data => data.Type == "UserId").Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MessageListByReceiverId(int id)
        {
            List<MessageDataGet> targetMessageData =
                (from messageData in _HouseMatcherContext.MessageData
                 where messageData.ReceiverId == id || messageData.SenderId == id
                 join sender in _HouseMatcherContext.UserData on messageData.SenderId equals sender.UserId
                 join receiver in _HouseMatcherContext.UserData on messageData.ReceiverId equals receiver.UserId
                 join houseData in _HouseMatcherContext.HouseData on messageData.HouseDataId equals houseData.HouseId
                 select new MessageDataGet
                 {
                     MessageId = messageData.MessageId,
                     MessageDescription = messageData.MessageDescription,
                     SenderId = messageData.SenderId,
                     SenderName = sender.UserName,
                     ReceiverId = messageData.ReceiverId,
                     ReceiverName = receiver.UserName,
                     CreatedTime = messageData.CreatedTime,
                     HouseDataId = messageData.HouseDataId,
                     HouseName = houseData.HouseName,
                     IsMyHouse = houseData.UserId.ToString() == UserId
                 }).ToList();

            return Ok(targetMessageData);
        }

        //儲存新訊息
        [HttpPost]
        public ActionResult NewMessage([FromBody] MessageDataPostDto messageData)
        {
            MessageData postMessageData = new MessageData()
            {
                MessageDescription = messageData.MessageDescription,
                SenderId = messageData.SenderId,
                ReceiverId = messageData.ReceiverId,
                HouseDataId = messageData.HouseDataId ,
                CreatedTime = DateTime.Now
            };
            _HouseMatcherContext.MessageData.Add(postMessageData);
            _HouseMatcherContext.SaveChanges();
            return Ok("訊息發送成功");
        }
    }
}
