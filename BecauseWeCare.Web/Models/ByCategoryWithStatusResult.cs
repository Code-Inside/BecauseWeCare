using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BecauseWeCare.Web.Models
{
    public class ByCategoryWithStatusResult
    {
        public List<KeyValuePair<string, int>> CategoriesAndCount { get; set; }
        public List<AnalyticsStatus> PerStatusInsight { get; set; }
    }
}