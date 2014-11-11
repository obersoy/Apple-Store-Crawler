using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class Consts
    {
        // Urls
        public const string ROOT_STORE_URL = "https://itunes.apple.com/us/genre/ios-books/id6018?mt=8";

        // Http Headers
        public const string USER_AGENT = "Crawling for a college work - more at https://github.com/MarcelloLins/AppStoreCrawler";
        
        // XPaths - Root Page 
        public const string XPATH_CATEGORIES_URLS = "//a[contains(@class,'top-level-genre')]";
        public const string XPATH_CHARACTERS_URLS = "//div[@id='selectedgenre']/ul[@class='list alpha']/li/a";
        public const string XPATH_NUMERIC_URLS    = "//ul[@class='list paginate']/li/a";
        public const string XPATH_NEXT_PAGE       = "//ul[@class='list paginate'][1]/li/a[@class='paginate-more']";
        public const string XPATH_LAST_PAGE       = "//ul[@class='list paginate'][1]/li/a[not(@class)]";
        public const string XPATH_APPS_URLS       = "//div[contains(@class,'column') and not(@id)]/ul/li/a";

        // XPaths - App Page
        public const string XPATH_TITLE            = "//div[@id='title']/div[@class='left']/h1";
        public const string XPATH_DEVELOPER_NAME   = "//div[@id='title']/div[@class='left']/h2";
        public const string XPATH_DEVELOPER_URL    = "//a[@class='view-more']";
        public const string XPATH_APP_PRICE        = "//ul[@class='list']/li/div[@class='price']";
        public const string XPATH_CATEGORY         = "//ul[@class='list']/li[@class='genre']/a";
        public const string XPATH_UPDATE_DATE      = "//ul[@class='list']/li[@class='release-date']";
        public const string XPATH_DESCRIPTION      = "//p[@class='truncate']";
        public const string XPATH_VERSION          = "//span[contains(text(),'Version')]";
        public const string XPATH_APP_SIZE         = "//span[contains(text(),'Size')]";
        public const string XPATH_THUMBNAIL        = "//div[@id='left-stack']//div//a//div[@class='artwork']/img";
        public const string XPATH_LANGUAGES        = "//ul[@class='list']/li[@class='language']";
        public const string XPATH_COMPATIBILITY    = "//span[@class='app-requirements']";
        public const string XPATH_MINIMUM_AGE      = "//div[@class='app-rating']/a";
        public const string XPATH_RATING_REASONS   = "//ul[@class='list app-rating-reasons']/li";
        public const string XPATH_RATINGS          = "//div[@class='extra-list customer-ratings']";
        public const string XPATH_IN_APP_PURCHASES = "//div[@class='extra-list in-app-purchases']";

        // Culture Info and Globalization
        public const string CURRENT_CULTURE_INFO = "en-US";
        public const string DATE_FORMAT          = "MMM dd, yyyy";

        // MongoDB - Remote Server
        public static readonly string MONGO_SERVER           = "ec2-54-88-152-45.compute-1.amazonaws.com"; 
        public static readonly string MONGO_PORT             = "21766";
        public static readonly string MONGO_USER             = "GitHubAppleCrawler";
        public static readonly string MONGO_PASS             = "g22LrJvULU5B";
        public static readonly string MONGO_DATABASE         = "AppleStore";
        public static readonly string MONGO_COLLECTION       = "AppleStoreApps";
        public static readonly string MONGO_AUTH_DB          = "AppleStore";
        public static readonly int    MONGO_TIMEOUT          = 10000;
    }
}
