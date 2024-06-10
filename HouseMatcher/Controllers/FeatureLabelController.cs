using HouseMatcher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureLabelController : ControllerBase
    {
        public readonly HouseMatcherContext _HouseMatcherContext;

        public FeatureLabelController(HouseMatcherContext houseMatcherContext)
        {
            _HouseMatcherContext = houseMatcherContext;
        }

        // POST api/<FileUploadController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_HouseMatcherContext.FeatureLabelList.ToList());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Post(string LabelName)
        {
            _HouseMatcherContext.FeatureLabelList.Add(new FeatureLabelList()
            {
                LabelId = new Guid(),
                LabelName = LabelName
            });
            _HouseMatcherContext.SaveChanges();
            return Ok("新增成功");
        }
    }
}
