using System;
using System.Linq;
using BecauseWeCare.Web.Models;
using Raven.Client.Indexes;

namespace BecauseWeCare.Web.Indexes
{
    public class ByDateIndex : AbstractIndexCreationTask<Suggestion, ByDateIndex.ByDateResult>
    {
        public class ByDateResult
        {
            public string Date { get; set; }
            public int AddedAtThisDate { get; set; }
            public DateTime ParsedDate { get; set; }
        }

        public ByDateIndex()
        {
            Map = suggestions => from suggestion in suggestions
                                 select new
                                            {
                                                Date = suggestion.created_at,
                                                ParsedDate = DateTime.Parse(suggestion.created_at),
                                                AddedAtThisDate = 1,
                                            };

            Reduce = results => from result in results
                                group result by new { month = result.ParsedDate.Month, year = result.ParsedDate.Year }
                                into g
                                select new
                                           {
                                               Date = new DateTime(g.Key.year, g.Key.month, 1),
                                               ParsedDate = new DateTime(g.Key.year, g.Key.month, 1),
                                               AddedAtThisDate = g.Sum(x => x.AddedAtThisDate),
                                           };

        }
    }
}