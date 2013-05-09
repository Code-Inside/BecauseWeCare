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

            SyncUserVoiceSiteToRavenDb(site, "http://localhost:8080/databases/feedbacks");

        }

        public static void SyncUserVoiceSiteToRavenDb(Site site, string ravenDbServer)
        {
            Console.WriteLine("SyncUserVoiceSite started for UserVoiceSite {0} to RavenDbServer {1}", site.Name, ravenDbServer);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var client = new UserVoice.Client(site.Subdomain, API_KEY);

            var suggestions = client.GetCollection("/api/v1/forums/" + site.Id + "/suggestions");

            var totalNumber = suggestions.Count;

            Console.WriteLine("Total suggestions: " + totalNumber);

            using (var documentStore = new DocumentStore { Url = ravenDbServer })
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
