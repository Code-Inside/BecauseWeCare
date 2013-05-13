using System.Collections.Generic;

namespace BecauseWeCare.Web.Models
{
    public class AnalyticsStatus
    {
        public string StatusName { get; set; }
        public string HexColor { get; set; }
        public List<int> Count { get; set; } 
    }
}