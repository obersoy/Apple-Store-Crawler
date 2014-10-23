using SharedLibrary;
using SharedLibrary.Logging;
using SharedLibrary.Parsing;
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
            AppStoreParser  parser     = new AppStoreParser ();
            _logger                    = new LogWrapper ();


            // Step 1 - Trying to obtain the root page html (source of all the apps)
            var rootPageResponse = httpClient.GetRootPage ();

            // Sanity Check
            if (String.IsNullOrWhiteSpace (rootPageResponse))
            {
                _logger.LogMessage ("Error obtaining Root Page HTMl - Aborting", "Timeout Error");
                return;
            }

            // Step 2 - Extracting Category Urls from the Root Page
            foreach (string categoryUrl in parser.ParseCategoryUrls (rootPageResponse))
            {
                
            }

            Console.Read();
        }
    }
}
