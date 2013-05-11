using System.Collections.Generic;

namespace BecauseWeCare.Web.Models
{
    public class ByCategoryWebResult
    {
        public List<string> AllStates { get; set; }
        public List<KeyValuePair<string, List<int>>> CategoryCountInStates { get; set; } 
    }
}
