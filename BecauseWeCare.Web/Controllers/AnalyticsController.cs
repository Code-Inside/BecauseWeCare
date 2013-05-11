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
        public List<ByCategoryIndex.ByCategoryResult> GetByCategory()
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "msftuservoice" })
            {
                documentStore.Initialize();

                IndexCreation.CreateIndexes(typeof(ByCategoryIndex).Assembly, documentStore);

                using (var session = documentStore.OpenSession())
                {
                    var resultByCategory = session.Query<ByCategoryIndex.ByCategoryResult, ByCategoryIndex>().ToList();

                    var result = new ByCategoryWebResult();
                    result.CategoryCountInStates = new List<KeyValuePair<string, List<int>>>();

                    foreach(var byCategory in resultByCategory)
                    {
                        foreach(var state in byCategory.States)
                        {
                            if (result.CategoryCountInStates.Any(x => x.Key == state.Name) == false)
                            {
                                result.CategoryCountInStates.Add(new KeyValuePair<string, List<int>>(state.Name, new List<int>()));
                            }
                        }
                    }

                    foreach (var categoryCountInStates in result.CategoryCountInStates)
                    {
                        foreach (var byCategoryResult in resultByCategory)
                        {
                            foreach (var categoryStates in byCategoryResult.States)
                            {
                            }
                        }
                    }

                    return resultByCategory;
                }
            }
        }
    }
}