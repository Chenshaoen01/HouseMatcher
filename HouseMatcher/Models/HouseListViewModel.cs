namespace HouseMatcher.Models
{
    public class HouseListViewModel
    {
        public List<HouseDataGetDto> HouseList { get; set; }

        public List<FeatureLabelList> FeatureList { get; set; }
    }

    public class HouseDetailViewModel
    {
        public List<HouseDataGetDto> HouseList { get; set; }

        public List<FeatureLabelList> FeatureList { get; set; }

        public UserData HouseOwnerName { get; set; }
    }
}