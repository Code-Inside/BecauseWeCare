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
        }

        public ByDateIndex()
        {
            Map = suggestions => from suggestion in suggestions
                                 select new
                                            {
                                                Date = suggestion.created_at,
                                                AddedAtThisDate = 1,
                                            };

            Reduce = results => from result in results
                                group result by result.Date
                                into g
                                select new
                                           {
                                               Date = g.Key,
                                               AddedAtThisDate = g.Sum(x => x.AddedAtThisDate),
                                           };

        }
    }
}