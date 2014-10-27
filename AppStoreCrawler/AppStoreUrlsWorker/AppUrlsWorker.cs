using SharedLibrary;
using SharedLibrary.AWS;
using SharedLibrary.ConfigurationReader;
using SharedLibrary.Logging;
using SharedLibrary.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStoreUrlsWorker
{
    class AppUrlsWorker
    {
        // Logging Tool
        private static LogWrapper _logger;

        // Configuration Values
        private static string _characterUrlsQueueName;
        private static string _appsUrlsQueueName;
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
            AWSSQSHelper charactersUrlQueue = new AWSSQSHelper (_characterUrlsQueueName, _maxMessagesPerDequeue, _awsKey, _awsKeySecret);
            AWSSQSHelper appsUrlsQueue      = new AWSSQSHelper (_appsUrlsQueueName     , _maxMessagesPerDequeue, _awsKey, _awsKeySecret);

            // Setting Error Flag to No Error ( 0 )
            System.Environment.ExitCode = 0;

            // Initialiazing Control Variables
            int fallbackWaitTime = 1;

            _logger.LogMessage ("Started Processing Category Urls");

            do
            {



            } while (true);

        }

        private static void LoadConfiguration ()
        {
            _maxRetries             = ConfigurationReader.LoadConfigurationSetting<int>    ("MaxRetries"           , 0);
            _maxMessagesPerDequeue  = ConfigurationReader.LoadConfigurationSetting<int>    ("MaxMessagesPerDequeue", 10);
            _appsUrlsQueueName      = ConfigurationReader.LoadConfigurationSetting<String> ("AppsUrlsQueueName"    , String.Empty);
            _characterUrlsQueueName = ConfigurationReader.LoadConfigurationSetting<String> ("AWSCharacterUrlsQueue", String.Empty);
            _awsKey                 = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKey"               , String.Empty);
            _awsKeySecret           = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKeySecret"         , String.Empty);
        }

    }
}
