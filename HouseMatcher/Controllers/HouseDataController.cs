using HouseMatcher.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseDataController : ControllerBase
    {
        public readonly HouseMatcherContext _HouseMatcherContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HouseDataController(HouseMatcherContext houseMatcherContext, IHttpContextAccessor httpContextAccessor)
        {
            _HouseMatcherContext = houseMatcherContext;
            _httpContextAccessor = httpContextAccessor;
        }
        //GET: api/<HouseDataController>
        [HttpGet]
        public ActionResult<IEnumerable<HouseDataGetDto>> GetAll(int Page, string? OrderByColumn)
        {
            List<HouseDataGetDto> HousDataList = new List<HouseDataGetDto>() { };
            int ItemsPerPage = 10;

            if (OrderByColumn == null)
            {
                HousDataList = _HouseMatcherContext.HouseData
                    .Select(HouseData => new HouseDataGetDto()
                    {
                        HouseId = HouseData.HouseId,
                        HouseName = HouseData.HouseName,
                        Description = HouseData.Description,
                        Location = HouseData.Location,
                        HouseSize = HouseData.HouseSize,
                        RentPerMonth = HouseData.RentPerMonth,
                        Latitude = HouseData.Latitude,
                        Longitude = HouseData.Longitude,
                        City = HouseData.City,
                        District = HouseData.District,
                        FeaturLabelList = _HouseMatcherContext.FeatureLabelDictionary
                           .Where(FeatureLabelDictionary => FeatureLabelDictionary.HouseDataId == HouseData.HouseId)
                           .Select(FeatureLabelDictionary => new FeatureLabelList()
                           {
                               LabelId = FeatureLabelDictionary.FeatureLabelId,
                               LabelName = FeatureLabelDictionary.FeatureLabelName
                           }).ToList()
                    })
                    .ToList();
            }
            else
            {
                HousDataList = _HouseMatcherContext.HouseData
                    .OrderBy(housedata => housedata.UpdateTime).Skip((Page - 1) * ItemsPerPage).Take(ItemsPerPage)
                    .Select(HouseData => new HouseDataGetDto()
                    {
                        HouseId = HouseData.HouseId,
                        HouseName = HouseData.HouseName,
                        Description = HouseData.Description,
                        Location = HouseData.Location,
                        HouseSize = HouseData.HouseSize,
                        RentPerMonth = HouseData.RentPerMonth,
                        Latitude = HouseData.Latitude,
                        Longitude = HouseData.Longitude,
                        City = HouseData.City,
                        District = HouseData.District,
                        FeaturLabelList = _HouseMatcherContext.FeatureLabelDictionary
                           .Where(FeatureLabelDictionary => FeatureLabelDictionary.HouseDataId == HouseData.HouseId)
                           .Select(FeatureLabelDictionary => new FeatureLabelList()
                           {
                               LabelId = FeatureLabelDictionary.FeatureLabelId,
                               LabelName = FeatureLabelDictionary.FeatureLabelName
                           }).ToList()
                    })
                    .ToList();
            }

            return HousDataList;
        }

        // GET api/<HouseDataController>/5
        [HttpGet("{id}")]
        public ActionResult GetById(Guid id)
        {
            List<HouseDataGetDto> targetHouseData = _HouseMatcherContext.HouseData
                .Where(houseData => houseData.HouseId == id)
                .Select(HouseData => new HouseDataGetDto()
                {
                    HouseId = HouseData.HouseId,
                    HouseName = HouseData.HouseName,
                    Description = HouseData.Description,
                    Location = HouseData.Location,
                    HouseSize = HouseData.HouseSize,
                    RentPerMonth = HouseData.RentPerMonth,
                    Latitude = HouseData.Latitude,
                    Longitude = HouseData.Longitude,
                    City = HouseData.City,
                    District = HouseData.District,
                    FeaturLabelList = _HouseMatcherContext.FeatureLabelDictionary
                           .Where(FeatureLabelDictionary => FeatureLabelDictionary.HouseDataId == HouseData.HouseId)
                           .Select(FeatureLabelDictionary => new FeatureLabelList()
                           {
                               LabelId = FeatureLabelDictionary.FeatureLabelId,
                               LabelName = FeatureLabelDictionary.FeatureLabelName
                           }).ToList()
                }).ToList();
            if(targetHouseData.Count ==0)
            {
                return NotFound("無此筆資料");
            }

            return Ok(targetHouseData);
        }

        // POST api/<HouseDataController>
        [HttpPost]
        public ActionResult Post([FromBody] HouseDataPostDto value)
        {
            Guid NewHouseDataId = new Guid();
            Guid NewUserId = new Guid();
            //新增租屋資料
            HouseData postHouseData = new HouseData()
            {
                HouseId = NewHouseDataId,
                HouseName = value.HouseName,
                Description = value.Description,
                Location = value.Location,
                HouseSize = value.HouseSize,
                RentPerMonth = value.RentPerMonth,
                Latitude = value.Latitude,
                Longitude = value.Longitude,
                City = value.City,
                District = value.District,
                UpdateTime = DateTime.Now
            };
            _HouseMatcherContext.HouseData.Add(postHouseData);

            //新增租屋特色
            List<FeatureLabelListDto> AddedFeatureLabel = value.FeaturLabelList.Where(FeaturLabel => FeaturLabel.EntitySate == "Added").ToList();
            foreach (var FeatureLabel in AddedFeatureLabel)
            {
                _HouseMatcherContext.FeatureLabelDictionary.Add(new FeatureLabelDictionary()
                {
                    DictionaryId = new Guid(),
                    FeatureLabelId = FeatureLabel.LabelId,
                    HouseDataId = postHouseData.HouseId,
                    FeatureLabelName = FeatureLabel.LabelName
                });
            }

            _HouseMatcherContext.SaveChanges();
            return Ok("新增成功");
        }

        // PUT api/<HouseDataController>/5
        [HttpPut]
        public ActionResult Put([FromBody] HouseDataPutDto value)
        {
            HouseData targetHouseData = _HouseMatcherContext.HouseData.Where(HouseData => HouseData.HouseId == value.HouseId).FirstOrDefault();

            targetHouseData.HouseName = value.HouseName;
            targetHouseData.Description = value.Description;
            targetHouseData.Location = value.Location;
            targetHouseData.HouseSize = value.HouseSize;
            targetHouseData.RentPerMonth = value.RentPerMonth;
            targetHouseData.Latitude = value.Latitude;
            targetHouseData.Longitude = value.Longitude;
            targetHouseData.City = value.City;
            targetHouseData.District = value.District;
            targetHouseData.UpdateTime = DateTime.Now;

            //新增租屋特色
            List<FeatureLabelListDto> AddedFeatureLabel = value.FeaturLabelList.Where(FeaturLabel => FeaturLabel.EntitySate == "Added").ToList();
            foreach (var FeatureLabel in AddedFeatureLabel)
            {
                _HouseMatcherContext.FeatureLabelDictionary.Add(new FeatureLabelDictionary()
                {
                    DictionaryId = new Guid(),
                    FeatureLabelId = FeatureLabel.LabelId,
                    HouseDataId = value.HouseId,
                    FeatureLabelName = FeatureLabel.LabelName
                });
            }

            ///刪除租屋特色
            IEnumerable<Guid> DeledtedFeatureLabelIds = value.FeaturLabelList
                 .Where(FeaturLabel => FeaturLabel.EntitySate == "Deleted")
                 .Select(FeaturLabel => FeaturLabel.LabelId);

             var DeletedTargetList = _HouseMatcherContext.FeatureLabelDictionary
                .Where(FeaturLabel => DeledtedFeatureLabelIds.Contains(FeaturLabel.FeatureLabelId) 
                && FeaturLabel.HouseDataId == value.HouseId).ToList();

            foreach (var FeatureLabel in DeletedTargetList)
            {
                _HouseMatcherContext.FeatureLabelDictionary.RemoveRange(FeatureLabel);
            }

            _HouseMatcherContext.SaveChanges();
            return Ok("更新成功");        }

        // DELETE api/<HouseDataController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var deleteTarget = _HouseMatcherContext.HouseData.Find(id);
            if(deleteTarget == null)
            {
                return NotFound("無此資料");
            }
            else
            {
                _HouseMatcherContext.HouseData.RemoveRange(deleteTarget);
                _HouseMatcherContext.SaveChanges();
                return Ok("刪除成功");
            }
        }
    }
}
