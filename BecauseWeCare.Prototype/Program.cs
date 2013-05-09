using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Client.Document;

namespace BecauseWeCare.Prototype
{
    class Program
    {
        static string API_KEY = ""; // Needed!

        static void Main(string[] args)
        {

            Site site = new Site();
            site.Id = "110705";
            site.Name = "Windows Phone Dev Platform";
            site.Subdomain = "wpdev";
            site.Url = "http://wpdev.uservoice.com/forums/110705-dev-platform";

            SyncUserVoiceSiteToRavenDb(site, "http://localhost:8080/", "msftuservoice");

        }

        public static void SyncUserVoiceSiteToRavenDb(Site site, string ravenDbServer, string database)
        {
            Console.WriteLine("SyncUserVoiceSite started for UserVoiceSite {0} to RavenDbServer {1}", site.Name, ravenDbServer + "@" + database);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var client = new UserVoice.Client(site.Subdomain, API_KEY);
            // for closed stuff we need the filter=closed (which is the only filter i can access ?? - otherwise just suggestions.json as endpoint
            var suggestions = client.GetCollection("/api/v1/forums/" + site.Id + "/suggestions.json?filter=closed");

            var totalNumber = suggestions.Count;

            Console.WriteLine("Total suggestions: " + totalNumber);

            using (var documentStore = new DocumentStore { Url = ravenDbServer, DefaultDatabase = database})
            {
                documentStore.Initialize();

                using (var bulkInsert = documentStore.BulkInsert(options: new BulkInsertOptions() { CheckForUpdates = true, BatchSize = 100 }))
                {
                    int i = 0;

                    foreach (var suggestionJson in suggestions)
                    {
                        int percentage = 0;

                        if(i > 0)
                        {
                            percentage =  i * 100/totalNumber;
                        }

                        Console.WriteLine("Completed {0}% (Counter: {1})", percentage, i);

                        var suggestionObject = suggestionJson.ToObject<Suggestion>();

                        suggestionObject.Site = site;
                        bulkInsert.Store(suggestionObject);
                        i++;
                    }
                }
            }

            watch.Stop();

            Console.WriteLine("Job for UserVoiceSite {0} finished. Elpased Time: {1}:{2}", site.Name, watch.Elapsed.Minutes, watch.Elapsed.Seconds);

        }
    }



}
