using System;
using System.Collections.Generic;

namespace HouseMatcher.Models
{
    public partial class FeatureLabelList
    {
        public Guid LabelId { get; set; }
        public string LabelName { get; set; } = null!;
    }
}
