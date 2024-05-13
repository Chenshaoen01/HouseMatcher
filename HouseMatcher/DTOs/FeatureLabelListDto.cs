using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class FeatureLabelListDto
    {
        public Guid LabelId { get; set; }
        public string LabelName { get; set; }
        public string EntitySate { get; set; }
    }
}
