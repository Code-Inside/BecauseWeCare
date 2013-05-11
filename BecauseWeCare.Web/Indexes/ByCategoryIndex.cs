using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;
using BecauseWeCare.Web.Models;

namespace BecauseWeCare.Web.Indexes
{
    /// <summary>
    /// Inspired by this sample: http://ayende.com/blog/158977/awesome-indexing-with-ravendb
    /// </summary>
    public class ByCategoryIndex : AbstractIndexCreationTask<Suggestion, ByCategoryIndex.ByCategoryResult>
    {
        public class ByCategoryResult
        {
            public string CategoryName { get; set; }
            public int Count { get; set; }
            public StatusStats[] States { get; set; }

            public class StatusStats
            {
                public string Name { get; set; }
                public int Count { get; set; }
                public string HexColor { get; set; }
            }
        }

        public ByCategoryIndex()
        {
            Map = suggestions => from suggestion in suggestions
                                 select new
                                 {
                                     CategoryName = suggestion.category.name ?? "",
                                     Count = 1,
                                     States = new[] { new { Name = suggestion.status.name, Count = 1, HexColor = suggestion.status.hex_color } },
                                 };

            Reduce = results => from result in results
                                group result by result.CategoryName
                                    into g
                                    select new
                                    {
                                        CategoryName = g.Key,
                                        Count = g.Sum(x => x.Count),
                                        States = from status in g.SelectMany(x => x.States)
                                                 group status by status.Name
                                                     into gb
                                                     select new { Name = gb.Key, Count = gb.Sum(x => x.Count), HexColor = gb.Select(x => x.HexColor).FirstOrDefault() }
                                    };

        }
    }
}