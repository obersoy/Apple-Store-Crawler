using SharedLibrary;
using SharedLibrary.Logging;
using SharedLibrary.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            #region ** Step 3 Action Handler **

            // Creating Action to Handle Step 3
            Action<String> categoriesScrapperAction = (string categoryUrl) =>
            {
                // Creating Thread-only instance of Requests Handler
                RequestsHandler threadHttpClient = new RequestsHandler();
                String categoryHtmlResponse;

                // Retrying Get Request
                int retriesCount = 0, maxRetries = 10;
                do
                {
                    // Executing Get for the URL received
                    categoryHtmlResponse = threadHttpClient.Get(categoryUrl);
                    retriesCount++;

                } while (String.IsNullOrWhiteSpace(categoryHtmlResponse) && retriesCount <= maxRetries);

                // Creating thread-only instance of Page Parser
                AppStoreParser scrapper = new AppStoreParser();
                var scp = scrapper.ParseCharacterUrls(categoryHtmlResponse).ToList();

                // Iterating over Parsed Character Urls (A,B,C...#)
                //scrapper.ParseCharacterUrls (categoryHtmlResponse).ToList().ForEach (categoriesScrapperAction);
            };

            #endregion

            Action<String> charactersUrlScrapperAction = (String characterUrl) =>
            {

            };

            // Step 1 - Trying to obtain the root page html (source of all the apps)
            var rootPageResponse = httpClient.GetRootPage ();

            // Sanity Check
            if (String.IsNullOrWhiteSpace (rootPageResponse))
            {
                _logger.LogMessage ("Error obtaining Root Page HTMl - Aborting", "Timeout Error");
                return;
            }

            

            // Step 2 - Extracting Category Urls from the Root Page
            parser.ParseCategoryUrls(rootPageResponse).ToList().ForEach(categoriesScrapperAction);

            Console.ReadLine ();
        }
    }
}
