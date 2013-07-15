using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BecauseWeCare.Model;
using Raven.Abstractions.Data;
using Raven.Client.Document;

namespace BecauseWeCare.Importer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            UserVoiceSite site = new UserVoiceSite();
            site.Id = "110705";
            site.Name = "Windows Phone Dev Platform";
            site.Subdomain = "wpdev";
            site.Url = "http://wpdev.uservoice.com/forums/110705-dev-platform";

            UserVoiceSite aspnet = new UserVoiceSite();
            aspnet.Id = "41199";
            aspnet.Name = "General ASP.NET";
            aspnet.Topic = "ASP.NET";
            aspnet.Subdomain = "aspnet";
            aspnet.Url = "http://aspnet.uservoice.com/forums/41199-general-asp-net";

            var ravenDbHost = ConfigurationManager.AppSettings["RavenDBHost"];
            var ravenDbDatabase = ConfigurationManager.AppSettings["RavenDBDatabase"];
            
            //SyncUserVoiceSiteToRavenDb(site, ravenDbHost, ravenDbDatabase);
            SyncUserVoiceSiteToRavenDb(aspnet, ravenDbHost, ravenDbDatabase);
        }

        public static void SyncUserVoiceSiteToRavenDb(UserVoiceSite site, string ravenDbServer, string database)
        {
            Console.WriteLine("SyncUserVoiceSite started for UserVoiceSite {0} to RavenDbServer {1}", site.Name,
                              ravenDbServer + "@" + database);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var client = new UserVoice.Client(site.Subdomain, apiKey: "", apiSecret: "");
            // for closed stuff we need the filter=closed (which is the only filter i can access ?? - otherwise just suggestions.json as endpoint
            var suggestions = client.GetCollection("/api/v1/forums/" + site.Id + "/suggestions.json");

            var totalNumber = suggestions.Count;

            Console.WriteLine("Total suggestions: " + totalNumber);

            int i = 0;
            
            using (var documentStore = new DocumentStore {Url = ravenDbServer, DefaultDatabase = database})
            {
                Console.WriteLine("Save Site MetaData {0}", site.Name);

                documentStore.Initialize();

                using(var session = documentStore.OpenSession())
                {
                    session.Store(site);
                    session.SaveChanges();
                }

                Console.WriteLine("... and BulkInsert!");

                using (var bulkInsert =
                        documentStore.BulkInsert(options: new BulkInsertOptions() {CheckForUpdates = true, BatchSize = 100}))
                {
                    foreach (var suggestionJson in suggestions)
                    {
                        int percentage = 0;

                        if (i > 0)
                        {
                            percentage = i*100/totalNumber;
                        }

                        Console.WriteLine("Completed {0}% (Counter: {1})", percentage, i);

                        var suggestion = suggestionJson.ToObject<Suggestion>();

                        suggestion.UserVoiceSiteId = site.Id;

                        bulkInsert.Store(suggestion);
                        i++;
                    }
                }
            }


            watch.Stop();

            Console.WriteLine("Job for UserVoiceSite {0} finished. Elpased Time: {1}:{2}", site.Name,
                              watch.Elapsed.Minutes, watch.Elapsed.Seconds);

        }

    }
}
