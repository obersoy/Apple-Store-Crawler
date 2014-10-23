using SharedLibrary;
using SharedLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStoreCrawler
{
    class Crawler
    {
        private static LogWrapper _logger;

        static void Main (string[] args)
        {
            // Creating Needed Instances
            RequestsHandler httpClient = new RequestsHandler ();
            _logger                    = new LogWrapper ();

            // Step 1 - Trying to obtain the root page html (source of all the apps)
            var resp = httpClient.GetRootPage ();

            // Sanity Check
            if (String.IsNullOrWhiteSpace (resp))
            {
                _logger.LogMessage ("Error obtaining Root Page HTMl - Aborting", "Timeout Error");
                return;
            }
        }
    }
}
