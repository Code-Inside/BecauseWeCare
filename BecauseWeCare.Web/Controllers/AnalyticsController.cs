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
        [System.Web.Http.HttpGet]
        public List<ByCategoryIndex.ByCategoryResult> ByCategory()
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "msftuservoice" })
            {
                documentStore.Initialize();

                IndexCreation.CreateIndexes(typeof(ByCategoryIndex).Assembly, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var resultByCategory = session.Query<ByCategoryIndex.ByCategoryResult, ByCategoryIndex>().ToList();

                    return resultByCategory;
                }
            }
        }

        [System.Web.Http.HttpGet]
        public ByCategoryWithStatusResult ByCategoryWithStatus()
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "msftuservoice" })
            {
                documentStore.Initialize();

                IndexCreation.CreateIndexes(typeof(ByCategoryIndex).Assembly, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var resultByCategory = session.Query<ByCategoryIndex.ByCategoryResult, ByCategoryIndex>().ToList();

                    var webResult = new ByCategoryWithStatusResult();
                    webResult.StatuslistPerCategory = new List<KeyValuePair<string, List<int>>>();

                    // Loop through each category to load all states
                    foreach(var byCategory in resultByCategory)
                    {
                        foreach(var state in byCategory.States)
                        {
                            if (webResult.StatuslistPerCategory.Any(x => x.Key == state.Name) == false)
                            {
                                webResult.StatuslistPerCategory.Add(new KeyValuePair<string, List<int>>(state.Name, new List<int>()));
                            }
                        }
                    }

                    webResult.StatuslistPerCategory = webResult.StatuslistPerCategory.OrderBy(x => x.Key).ToList();

                    // Loop again through each category and append "unused" states to create the correct diagram
                    foreach (var byCategory in resultByCategory)
                    {
                        var attachedStats = byCategory.States.ToList();

                        foreach (var status in webResult.StatuslistPerCategory)
                        {
                            if(attachedStats.Any(x => x.Name == status.Key) == false)
                            {
                                attachedStats.Add(new ByCategoryIndex.ByCategoryResult.StatusStats() { Count = 0, Name = status.Key});
                            }
                        }

                        byCategory.States = attachedStats.OrderBy(x => x.Name).ToArray();
                    }

                    // We need a list of States with the count from each category
                        foreach (var byCategory in resultByCategory)
                        {
                            foreach (var statesFromCategory in byCategory.States)
                            {
                                var indexInAllStates = webResult.StatuslistPerCategory.FindIndex(x => x.Key == statesFromCategory.Name);
                                webResult.StatuslistPerCategory[indexInAllStates].Value.Add(statesFromCategory.Count);
                            }
                        }

                    webResult.Categories = resultByCategory.Select(x => x.CategoryName).ToList();

                    return webResult;
                }
            }
        }
    }
}