using HouseMatcher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace HouseMatcher.Controllers
{
    public class HouseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly HouseMatcherContext _HouseMatcherContext;
        private int UserId;

        public HouseController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, HouseMatcherContext houseMatcherContext)
        {
            _logger = logger;
            _HouseMatcherContext = houseMatcherContext;
            Claim UserIdString = httpContextAccessor.HttpContext.User.FindFirst(data => data.Type == "UserId");
            if (UserIdString != null)
            {
                UserId = Convert.ToInt32(UserIdString.Value);
            }
        }

        //表單頁面
        public IActionResult FormPage()
        {
            return View();
        }

        //地圖搜尋頁面
        public IActionResult SearchMap(string? Disctict, int? MaxRent, int? MinRent, string? Feature)
        {
            HouseListViewModel viewModel = new HouseListViewModel();
            viewModel.FeatureList = GetFeatureLabelList();

            //取得符合條件的房屋資料
            List<HouseData> houseDataResult = getHouseDataMatch(Disctict, MaxRent, MinRent, Feature);

            //將房屋資料轉換為 HouseDataGetDto 並寫入 viewModel
            viewModel.HouseList = getHouseDataGetList(houseDataResult);

            return View(viewModel);
        }

        //詳細租屋資訊頁面
        public IActionResult Detail(Guid Id)
        {
            HouseListViewModel viewModel = new HouseListViewModel();
            viewModel.FeatureList = GetFeatureLabelList();

            List<HouseData> houseDataResult = _HouseMatcherContext.HouseData
                .Where(houseData => houseData.HouseId == Id)
                .Select(houseData => houseData).ToList();
            viewModel.HouseList = getHouseDataGetList(houseDataResult);

            ViewBag.IsEdit = true;
            return View(viewModel);
        }

        //各帳戶租屋列表管理頁面
        public IActionResult ManageList(int Page)
        {
            HouseListViewModel viewModel = new HouseListViewModel();
            viewModel.FeatureList = GetFeatureLabelList();

            //取得符合 UserId 的房屋資料
            List<HouseData> houseDataResult = _HouseMatcherContext.HouseData
                .Where(HouseData => HouseData.UserId == UserId)
                .Select(houseData => houseData).ToList();

            //篩選出符合頁數的資料
            int ItemsPerPage = 5;
            List<HouseData> houseDataResultMatcgPage = houseDataResult.Skip((Page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();

            viewModel.HouseList = getHouseDataGetList(houseDataResultMatcgPage);

            //ViewBag 寫入目前頁數
            ViewBag.CurrentPage = Page;
            //ViewBag 寫入最後一頁頁數
            float PageCountQuotient = (float)houseDataResult.Count() / (float)ItemsPerPage;
            ViewBag.LastPage = Math.Ceiling(PageCountQuotient);

            ViewBag.IsEdit = false;
            return View(viewModel);
        }

        //搜尋頁面
        public IActionResult Search(int Page, string? Disctict, int? MaxRent, int? MinRent, string? Feature)
        {
            HouseListViewModel viewModel = new HouseListViewModel();
            viewModel.FeatureList = GetFeatureLabelList();

            //取得符合條件的房屋資料
            List<HouseData> houseDataResult = getHouseDataMatch(Disctict, MaxRent, MinRent, Feature);

            //篩選出符合頁數的資料
            int ItemsPerPage = 5;
            List<HouseData> houseDataResultMatcgPage = houseDataResult.Skip((Page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();

            //將房屋資料轉換為 HouseDataGetDto 並寫入 viewModel
            viewModel.HouseList = getHouseDataGetList(houseDataResultMatcgPage);

            //ViewBag 寫入目前頁數
            ViewBag.CurrentPage = Page;
            //ViewBag 寫入最後一頁頁數
            float PageCountQuotient = (float)houseDataResult.Count() / (float)ItemsPerPage;
            ViewBag.LastPage = Math.Ceiling(PageCountQuotient);

            return View(viewModel);
        }

        public List<HouseData> getHouseDataMatch(string? Disctict, int? MaxRent, int? MinRent, string? Feature)
        {
            //建立區域篩選條件列表
            List<string> DisctictList = Disctict == null ? new List<string>() : Disctict.Split('+').ToList();

            //建立房屋特色條件列表
            List<string> FeatureIdList = Feature == null ? new List<string>() : Feature.Split('+').ToList();
            List<string> HouseIdListMatchFeatureIdList = GetHouseIdListMatchFeatureIdList(FeatureIdList);

            List<HouseData> houseDataResult = _HouseMatcherContext.HouseData
            .Where(houseData => DisctictList.Count() == 0 ? true : DisctictList.Contains(houseData.District))
            .Where(houseData => MaxRent == null ? true : MaxRent >= houseData.RentPerMonth)
            .Where(houseData => MinRent == null ? true : MinRent <= houseData.RentPerMonth)
            .Where(houseData => FeatureIdList.Count() == 0 ? true : HouseIdListMatchFeatureIdList.Contains(houseData.HouseId.ToString()))
            .ToList();

            return houseDataResult;
        }

        public List<string> GetHouseIdListMatchFeatureIdList(List<string> FeatureIdList)
        {
            List<FeatureLabelDictionary> FeatureLabelDictionaryData = FeatureIdList.Count() == 0 ? new List<FeatureLabelDictionary>() :
                _HouseMatcherContext.FeatureLabelDictionary
                .Where(featureLabelDictionaryItem => FeatureIdList.Contains(featureLabelDictionaryItem.FeatureLabelId.ToString()))
                .Select(featureLabelDictionaryItem => featureLabelDictionaryItem)
                .ToList();

            List<string> HouseIdListMatchFeatureIdList = new List<string>();

            if (FeatureIdList.Count() > 0)
            {
                foreach (var FeatureDictionaryItem in FeatureLabelDictionaryData)
                {
                    bool isNewHouseId = HouseIdListMatchFeatureIdList.FindIndex(
                        currentHouseId => FeatureDictionaryItem.HouseDataId.ToString() == currentHouseId) == -1;

                    if (isNewHouseId)
                    {
                        bool isMatchFeatureList = FeatureIdList.All(
                            FeatureId => FeatureLabelDictionaryData.Any(featureLabelDictionaryItem =>
                                featureLabelDictionaryItem.HouseDataId == FeatureDictionaryItem.HouseDataId
                                && featureLabelDictionaryItem.FeatureLabelId.ToString() == FeatureId
                        ));
                        if (isMatchFeatureList)
                        {
                            HouseIdListMatchFeatureIdList.Add(FeatureDictionaryItem.HouseDataId.ToString());
                        }
                    }
                }
            }

            return HouseIdListMatchFeatureIdList;
        }


        public List<HouseDataGetDto> getHouseDataGetList(List<HouseData> HouseDataList)
        {
            List<FeatureLabelDictionary> FeatureLabelDictionaryData = _HouseMatcherContext.FeatureLabelDictionary.ToList();

            List<HouseDataGetDto> HouseDataGetDtoList = new List<HouseDataGetDto>();
            foreach (var HouseDataItem in HouseDataList)
            {
                HouseDataGetDto newHouseData = new HouseDataGetDto()
                {
                    HouseId = HouseDataItem.HouseId,
                    UserId = HouseDataItem.UserId,
                    HouseName = HouseDataItem.HouseName,
                    Description = HouseDataItem.Description,
                    Location = HouseDataItem.Location,
                    HouseSize = HouseDataItem.HouseSize,
                    RentPerMonth = HouseDataItem.RentPerMonth,
                    Latitude = HouseDataItem.Latitude,
                    Longitude = HouseDataItem.Longitude,
                    City = HouseDataItem.City,
                    District = HouseDataItem.District,
                    Road = HouseDataItem.Road,
                    FeaturLabelList = new List<FeatureLabelList>(),
                    HouseImg = HouseDataItem.HouseImg
                };

                foreach (var featureLabelDictionaryItem in FeatureLabelDictionaryData)
                {
                    if (featureLabelDictionaryItem.HouseDataId == HouseDataItem.HouseId)
                        newHouseData.FeaturLabelList.Add(new FeatureLabelList()
                        {
                            LabelId = featureLabelDictionaryItem.FeatureLabelId,
                            LabelName = featureLabelDictionaryItem.FeatureLabelName
                        });
                }

                HouseDataGetDtoList.Add(newHouseData);
            }

            
            return HouseDataGetDtoList;
        }

        //新增房屋資訊
        [HttpPost]
        public ActionResult HousePost([FromBody] HouseDataPostDto value)
        {
            Guid NewHouseDataId = new Guid();
            Guid NewUserId = new Guid();
            //新增租屋資料
            HouseData postHouseData = new HouseData()
            {
                HouseId = NewHouseDataId,
                UserId = UserId,
                HouseName = value.HouseName,
                Description = value.Description,
                Location = value.Location,
                HouseSize = value.HouseSize,
                RentPerMonth = value.RentPerMonth,
                Latitude = value.Latitude,
                Longitude = value.Longitude,
                City = value.City,
                District = value.District,
                Road = value.Road,
                UpdateTime = DateTime.Now,
                HouseImg = value.HouseImg
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

        //修改房屋資訊
        [HttpPut]
        public ActionResult HousePut([FromBody] HouseDataPutDto value)
        {
            string name = value.HouseName;
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
            targetHouseData.Road = value.Road;
            targetHouseData.UpdateTime = DateTime.Now;
            targetHouseData.HouseImg = value.HouseImg;

            //新增租屋特色
            List<FeatureLabelListDto> AddedFeatureLabel = value.FeaturLabelList
                .Where(FeaturLabel => FeaturLabel.EntitySate == "Added").ToList();
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
            return Ok("更新成功");
        }

        [HttpDelete]
        public ActionResult HouseDelete(Guid id)
        {
            var HouseDeleteTarget = _HouseMatcherContext.HouseData.Find(id);
            var FeatureDeleteTarget = _HouseMatcherContext.FeatureLabelDictionary.Where(feature => feature.HouseDataId == id);
            if (HouseDeleteTarget == null)
            {
                return NotFound("無此資料");
            }
            else
            {
                //刪除房屋特色資料
                foreach (var FeatureLabel in FeatureDeleteTarget)
                {
                    _HouseMatcherContext.FeatureLabelDictionary.RemoveRange(FeatureLabel);
                }

                //刪除房屋資料
                _HouseMatcherContext.HouseData.RemoveRange(HouseDeleteTarget);
                _HouseMatcherContext.SaveChanges();
                return Ok("刪除成功");
            }
        }

        public List<FeatureLabelList> GetFeatureLabelList()
        {
            List<FeatureLabelList> FeatureLabelList = _HouseMatcherContext.FeatureLabelList.ToList();
            return FeatureLabelList;
        }
    }
}