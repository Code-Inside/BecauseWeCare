using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using BecauseWeCare.Web.Indexes;
using BecauseWeCare.Web.Models;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace BecauseWeCare.Web.Controllers
{
    public class AnalyticsController : ApiController
    {
        private static DocumentStore GetDocumentStore()
        {
            return new DocumentStore() { Url = "http://localhost:8080/", DefaultDatabase = "msftuservoice" };
        }

        [System.Web.Http.HttpGet]
        public AnalyticsByCategoryResult ByCategory()
        {
            using (var documentStore = GetDocumentStore())
            {
                documentStore.Initialize();

                IndexCreation.CreateIndexes(typeof(ByCategoryIndex).Assembly, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var resultByCategory = session.Query<ByCategoryIndex.ByCategoryResult, ByCategoryIndex>().ToList();

                    var webResult = new AnalyticsByCategoryResult();

                    webResult.PerStatusInsight = new List<AnalyticsStatus>();

                    // Loop through each category to load all states
                    foreach(var byCategory in resultByCategory)
                    {
                        foreach(var state in byCategory.States)
                        {
                            if (webResult.PerStatusInsight.Any(x => x.StatusName == state.Name) == false)
                            {
                                webResult.PerStatusInsight.Add(new AnalyticsStatus() { StatusName = state.Name, HexColor = state.HexColor, Count = new List<int>()} );
                            }
                        }
                    }

                    webResult.PerStatusInsight = webResult.PerStatusInsight.OrderBy(x => x.StatusName).ToList();

                    // Loop again through each category and append "unused" states to create the correct diagram
                    foreach (var byCategory in resultByCategory)
                    {
                        var attachedStats = byCategory.States.ToList();

                        foreach (var status in webResult.PerStatusInsight)
                        {
                            if(attachedStats.Any(x => x.Name == status.StatusName) == false)
                            {
                                attachedStats.Add(new ByCategoryIndex.ByCategoryResult.StatusStats() { Count = 0, Name = status.StatusName});
                            }
                        }

                        byCategory.States = attachedStats.OrderBy(x => x.Name).ToArray();
                    }

                    // We need a list of States with the count from each category
                        foreach (var byCategory in resultByCategory)
                        {
                            foreach (var statesFromCategory in byCategory.States)
                            {
                                var indexInAllStates = webResult.PerStatusInsight.FindIndex(x => x.StatusName == statesFromCategory.Name);
                                webResult.PerStatusInsight[indexInAllStates].Count.Add(statesFromCategory.Count);
                            }
                        }

                    webResult.CategoriesAndCount = resultByCategory.Select(x => new KeyValuePair<string, int>(x.CategoryName, x.Count)).ToList();

                    return webResult;
                }
            }
        }
    }
}