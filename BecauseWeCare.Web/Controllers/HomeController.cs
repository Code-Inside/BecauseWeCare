using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BecauseWeCare.Web.Indexes;
using BecauseWeCare.Web.Models;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace BecauseWeCare.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "msftuservoice" })
            {
                documentStore.Initialize();

                IndexCreation.CreateIndexes(typeof(ByCategoryIndex).Assembly, documentStore);

               using(var session = documentStore.OpenSession())
               {
                   RavenQueryStatistics totalStats;
                   var resultTotal = session.Query<Suggestion>().
                       Statistics(out totalStats).
                       ToArray();

                   ViewBag.Total = totalStats.TotalResults;

                   var resultByStats = session.Query<ByStatusIndex.ByStatusResult, ByStatusIndex>().ToList();
                   var resultByDate = session.Query<ByDateIndex.ByDateResult, ByDateIndex>().ToList();


                   string foobar = resultByDate.ToString();
               }
            }


            return View();
        }
    }
}
