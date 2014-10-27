using SharedLibrary.ConfigurationReader;
using SharedLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStoreCategoriesWorker
{
    class CategoriesWorker
    {
        // Logging Tool
        private static LogWrapper _logger;

        // Configuration Values
        private static string _categoriesQueueName;
        private static string _awsKey;
        private static string _awsKeySecret;

        static void Main(string[] args)
        {
        }

        private static void LoadConfiguration ()
        {
            _categoriesQueueName = ConfigurationReader.LoadConfigurationSetting<String> ("AWSCategoriesQueue", String.Empty);
            _awsKey              = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKey"            , String.Empty);
            _awsKeySecret        = ConfigurationReader.LoadConfigurationSetting<String> ("AWSKeySecret"      , String.Empty);
        }
    }
}
