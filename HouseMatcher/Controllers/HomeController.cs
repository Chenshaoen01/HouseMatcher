using HouseMatcher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace HouseMatcher.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly HouseMatcherContext _HouseMatcherContext;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, HouseMatcherContext houseMatcherContext)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _HouseMatcherContext = houseMatcherContext;
        }

        public IActionResult Index()
        {
            HouseListViewModel viewModel = new HouseListViewModel();
            viewModel.HouseList = _HouseMatcherContext.HouseData
                    .OrderBy(housedata => housedata.UpdateTime).Take(3)
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

            viewModel.FeatureList = GetFeatureLabelList();
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            string UserId = _httpContextAccessor.HttpContext.User.FindFirst(data => data.Type == "UserId").Value;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public List<FeatureLabelList> GetFeatureLabelList()
        {
            List<FeatureLabelList> FeatureLabelList = _HouseMatcherContext.FeatureLabelList.ToList();
            return FeatureLabelList;
        }
    }
}