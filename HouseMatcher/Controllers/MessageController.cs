using HouseMatcher.Models;
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
    public class MessageController : Controller
    {
        public readonly HouseMatcherContext _HouseMatcherContext;
        public readonly IConfiguration _configuration;
        public MessageController(HouseMatcherContext houseMatcherContext, IConfiguration configuration)
        {
            _HouseMatcherContext = houseMatcherContext;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MessageListByReceiverId(int id)
        {
            List<MessageData> targetMessageData = _HouseMatcherContext.MessageData
                .Where(MessageData => MessageData.ReceiverId == id || MessageData.SenderId == id).ToList();

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
                CreatedTime = DateTime.Now
            };
            _HouseMatcherContext.MessageData.Add(postMessageData);
            _HouseMatcherContext.SaveChanges();
            return Ok("訊息發送成功");
        }
    }
}
