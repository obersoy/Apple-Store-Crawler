using SharedLibrary;
using SharedLibrary.AWS;
using SharedLibrary.ConfigurationReader;
using SharedLibrary.Logging;
using SharedLibrary.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AppStoreCategoriesWorker
{
    class CategoriesWorker
    {
        // Logging Tool
        private static LogWrapper _logger;

        // Configuration Values
        private static string _categoriesQueueName;
        private static string _characterUrlsQueueName;
        private static string _awsKey;
        private static string _awsKeySecret;
        private static int    _maxRetries;
        private static int    _maxMessagesPerDequeue;

        // Control Variables
        private static int    _hiccupTime = 1000;

        static void Main(string[] args)
        {
            // Creating Needed Instances
            RequestsHandler httpClient = new RequestsHandler ();
            AppStoreParser  parser     = new AppStoreParser ();
            _logger                    = new LogWrapper ();

            // Loading Configuration
            _logger.LogMessage ("Loading Configurations from App.config");
            LoadConfiguration ();

            // AWS Queue Handler
            _logger.LogMessage ("Initializing Queues");
            AWSSQSHelper categoriesUrlQueue = new AWSSQSHelper (_categoriesQueueName   , 10, _awsKey, _awsKeySecret);
            AWSSQSHelper charactersUrlQueue = new AWSSQSHelper (_characterUrlsQueueName, 10, _awsKey, _awsKeySecret);

            // Setting Error Flag to No Error ( 0 )
            System.Environment.ExitCode = 0;

            // Initialiazing Control Variables
            int fallbackWaitTime = 1;

            _logger.LogMessage ("Started Processing Category Urls");

            do
            {
                try
                {
                    // Dequeueing messages from the Queue
                    if (!categoriesUrlQueue.DeQueueMessages())
                    {
                        Thread.Sleep (_hiccupTime); // Hiccup                   
                        continue;
                    }

                    // Checking for no message received, and false positives situations
                    if (!categoriesUrlQueue.AnyMessageReceived())
                    {
                        // If no message was found, increases the wait time
                        int waitTime;
                        if (fallbackWaitTime <= 12)
                        {
                            // Exponential increase on the wait time, truncated after 12 retries
                            waitTime = Convert.ToInt32 (Math.Pow (2, fallbackWaitTime) * 1000);
                        }
                        else // Reseting Wait after 12 fallbacks
                        {
                            waitTime         = 2000;
                            fallbackWaitTime = 0;
                        }

                        fallbackWaitTime++;

                        // Sleeping before next try
                        Console.WriteLine ("Fallback (seconds) => " + waitTime);
                        Thread.Sleep (waitTime);
                        continue;
                    }

                    // Reseting fallback time
                    fallbackWaitTime = 1;

                    // Iterating over dequeued Messages
                    foreach (var categoryUrl in categoriesUrlQueue.GetDequeuedMessages())
                    {
                        // Console Feedback
                        _logger.LogMessage ("Started Parsing Category : " + categoryUrl.Body);

                        try
                        {
                            // Retries Counter
                            int retries = 0;
                            string htmlResponse;

                            // Retrying if necessary
                            do
                            {
                                // Executing Http Request for the Category Url
                                htmlResponse = httpClient.Get (categoryUrl.Body);

                                if (String.IsNullOrEmpty (htmlResponse))
                                {
                                    _logger.LogMessage ("Retrying Request for Category Page", "Request Error", BDC.BDCCommons.TLogEventLevel.Error);
                                    retries++;
                                }

                            } while (String.IsNullOrWhiteSpace (htmlResponse) && retries <= _maxRetries);

                            // Checking if retries failed
                            if (String.IsNullOrWhiteSpace (htmlResponse))
                            {
                                // Deletes Message and moves on
                                categoriesUrlQueue.DeleteMessage (categoryUrl);
                                continue;
                            }

                            // If the request worked, parses the urls out of the page
                            foreach (string characterUrls in parser.ParseCharacterUrls (htmlResponse))
                            {
                                // Enqueueing Urls
                                charactersUrlQueue.EnqueueMessage (HttpUtility.HtmlDecode (characterUrls));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogMessage(ex.Message, "Category Url Processing", BDC.BDCCommons.TLogEventLevel.Error);
                        }
                        finally
                        {
                            // Deleting the message
                            categoriesUrlQueue.DeleteMessage(categoryUrl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogMessage (ex);
                }

            } while (true);
        }

        private static void LoadConfiguration ()
        {
            _maxRetries             = ConfigurationReader.LoadConfigurationSetting<int>    ("MaxRetries"           , 0);
            _maxMessagesPerDequeue  = ConfigurationReader.LoadConfigurationSetting<int>    ("MaxMessagesPerDequeue", 10);
            _categoriesQueueName    = ConfigurationReader.LoadConfigurationSetting<String> ("AWSCategoriesQueue"   , String.Empty);
            _characterUrlsQueueName = ConfigurationReader.LoadConfigurationSetting<String> ("AWSCharacterUrlsQueue", String.Empty);
            _awsKey                 = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKey"               , String.Empty);
            _awsKeySecret           = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKeySecret"         , String.Empty);
        }
    }
}
