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

namespace AppStoreNumericsWorker
{
    class NumericsWorker
    {
        // Logging Tool
        private static LogWrapper _logger;

        // Configuration Values
        private static string _numericUrlsQueueName;
        private static string _appUrlsQueueName;
        private static string _awsKey;
        private static string _awsKeySecret;
        private static int    _maxRetries;
        private static int    _maxMessagesPerDequeue;

        // Control Variables
        private static int    _hiccupTime = 1000;

        static void Main (string[] args)
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
            AWSSQSHelper numericUrlQueue   = new AWSSQSHelper (_numericUrlsQueueName , _maxMessagesPerDequeue, _awsKey, _awsKeySecret);
            AWSSQSHelper appsUrlQueue      = new AWSSQSHelper (_appUrlsQueueName     , _maxMessagesPerDequeue, _awsKey, _awsKeySecret);

            // Setting Error Flag to No Error ( 0 )
            System.Environment.ExitCode = 0;

            // Initialiazing Control Variables
            int fallbackWaitTime = 1;

            _logger.LogMessage ("Started Processing Numeric Urls");

            do
            {
                try
                {
                    // Dequeueing messages from the Queue
                    if (!numericUrlQueue.DeQueueMessages ())
                    {
                        Thread.Sleep (_hiccupTime); // Hiccup                   
                        continue;
                    }

                    // Checking for no message received, and false positives situations
                    if (!numericUrlQueue.AnyMessageReceived ())
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
                    foreach (var numericUrl in numericUrlQueue.GetDequeuedMessages ())
                    {
                        try
                        {
                            // Retries Counter
                            int retries = 0;
                            string htmlResponse;

                            // Retrying if necessary
                            do
                            {
                                // Executing Http Request for the Category Url
                                htmlResponse = httpClient.Get (numericUrl.Body);

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
                                numericUrlQueue.DeleteMessage (numericUrl);
                                continue;
                            }

                            // Feedback
                            _logger.LogMessage ("Current page " + numericUrl.Body, "Extracting App Urls");

                            foreach (var parsedAppUrl in parser.ParseAppsUrls (htmlResponse))
                            {
                                // Enqueueing App Urls
                                appsUrlQueue.EnqueueMessage (HttpUtility.HtmlDecode (parsedAppUrl));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogMessage (ex.Message, "Category Numeric Processing", BDC.BDCCommons.TLogEventLevel.Error);
                        }
                        finally
                        {
                            // Deleting the message
                            numericUrlQueue.DeleteMessage (numericUrl);
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
            _numericUrlsQueueName   = ConfigurationReader.LoadConfigurationSetting<String> ("AWSNumericUrlsQueue"  , String.Empty);
            _appUrlsQueueName       = ConfigurationReader.LoadConfigurationSetting<String> ("AWSAppUrlsQueue"      , String.Empty);
            _awsKey                 = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKey"               , String.Empty);
            _awsKeySecret           = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKeySecret"         , String.Empty);
        }
    }
}
