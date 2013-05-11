using System.Linq;
using BecauseWeCare.Web.Models;
using Raven.Client.Indexes;

namespace BecauseWeCare.Web.Indexes
{
    public class ByStatusIndex : AbstractIndexCreationTask<Suggestion, ByStatusIndex.ByStatusResult>
    {
        public class ByStatusResult
        {
            public string StatusName { get; set; }
            public int Count { get; set; }
            public CategoryStats[] Categories { get; set; }

            public class CategoryStats
            {
                public string Name { get; set; }
                public int Count { get; set; }
            }
        }

        public ByStatusIndex()
        {
            Map = suggestions => from suggestion in suggestions
                                 select new
                                            {
                                                StatusName = suggestion.status.name ?? "",
                                                Count = 1,
                                                Categories = new[] { new { Name = suggestion.category.name, Count = 1 } }
                                            };

            Reduce = results => from result in results
                                group result by result.StatusName
                                into g
                                select new
                                           {
                                               StatusName = g.Key,
                                               Count = g.Sum(x => x.Count),
                                               Categories = from category in g.SelectMany(x => x.Categories)
                                                        group category by category.Name
                                                        into gb
                                                        select new { Name = gb.Key, Count = gb.Sum(x => x.Count) }
                                           };

        }
    }
}