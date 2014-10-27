using SharedLibrary;
using SharedLibrary.Logging;
using SharedLibrary.Parsing;
using System;
using SharedLibrary.AWS;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SharedLibrary.ConfigurationReader;

namespace AppStoreCrawler
{
    class Crawler
    {
        // Logging Tool
        private static LogWrapper _logger;

        // Configuration Values
        private static string _categoriesQueueName;
        private static string _awsKey;
        private static string _awsKeySecret;

        static void Main (string[] args)
        {
            // Creating Needed Instances
            RequestsHandler httpClient = new RequestsHandler ();
            AppStoreParser  parser     = new AppStoreParser ();
            _logger                    = new LogWrapper ();

            // Loading Configuration
            _logger.LogMessage ("Reading Configuration");
            LoadConfiguration ();

            // AWS Queue Handler
            _logger.LogMessage ("Initializing Queues");
            AWSSQSHelper sqsWrapper = new AWSSQSHelper (_categoriesQueueName, 10, _awsKey, _awsKeySecret);

            // Step 1 - Trying to obtain the root page html (source of all the apps)
            var rootPageResponse = httpClient.GetRootPage ();

            // Sanity Check
            if (String.IsNullOrWhiteSpace (rootPageResponse))
            {
                _logger.LogMessage ("Error obtaining Root Page HTMl - Aborting", "Timeout Error");
                return;
            }

            // Step 2 - Extracting Category Urls from the Root Page and queueing their Urls
            foreach (var categoryUrl in parser.ParseCategoryUrls (rootPageResponse))
            {
                // Logging Feedback
                _logger.LogMessage ("Queueing Category : " + categoryUrl);

                // Queueing Category Urls
                sqsWrapper.EnqueueMessage (categoryUrl);
            }

            _logger.LogMessage ("End of Bootstrapping phase");
        }

        private static void LoadConfiguration ()
        {
            _categoriesQueueName = ConfigurationReader.LoadConfigurationSetting<String> ("AWSCategoriesQueue", String.Empty);
            _awsKey              = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKey"            , String.Empty);
            _awsKeySecret        = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKeySecret"      , String.Empty);
        }
    }
}
