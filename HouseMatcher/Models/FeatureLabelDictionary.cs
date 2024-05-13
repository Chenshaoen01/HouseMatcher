using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class FeatureLabelDictionary
    {
        public Guid DictionaryId { get; set; }
        public Guid FeatureLabelId { get; set; }
        public Guid HouseDataId { get; set; }
        public string FeatureLabelName { get; set; }
    }
}
