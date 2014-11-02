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
        
        // XPaths - Root Page 
        public const string XPATH_CATEGORIES_URLS = "//a[contains(@class,'top-level-genre')]";
        public const string XPATH_CHARACTERS_URLS = "//div[@id='selectedgenre']/ul[@class='list alpha']/li/a";
        public const string XPATH_NUMERIC_URLS    = "//ul[@class='list paginate']/li/a";
        public const string XPATH_NEXT_PAGE       = "//ul[@class='list paginate'][1]/li/a[@class='paginate-more']";
        public const string XPATH_LAST_PAGE       = "//ul[@class='list paginate'][1]/li/a[not(@class)]";
        public const string XPATH_APPS_URLS       = "//div[contains(@class,'column') and not(@id)]/ul/li/a";

        // XPaths - App Page
        public const string XPATH_TITLE          = "//div[@id='title']/div[@class='left']/h1";
        public const string XPATH_DEVELOPER_NAME = "//div[@id='title']/div[@class='left']/h2";
        public const string XPATH_DEVELOPER_URL  = "//a[@class='view-more']";
        public const string XPATH_APP_PRICE      = "//ul[@class='list']/li/div[@class='price']";
        public const string XPATH_CATEGORY       = "//ul[@class='list']/li[@class='genre']/a";
        public const string XPATH_UPDATE_DATE    = "//ul[@class='list']/li[@class='release-date']";
        public const string XPATH_DESCRIPTION    = "//p[@class='truncate']";
        public const string XPATH_VERSION        = "//span[contains(text(),'Version')]";
        public const string XPATH_APP_SIZE       = "//span[contains(text(),'Size')]";
        public const string XPATH_THUMBNAIL      = "//div[@class='artwork']/img[@width='175']";
        public const string XPATH_LANGUAGES      = "//ul[@class='list']/li[@class='language']";
        public const string XPATH_COMPATIBILITY  = "//span[@class='app-requirements']";
        public const string XPATH_MINIMUM_AGE    = "//div[@class='app-rating']/a";

        // Culture Info and Globalization
        public const string CURRENT_CULTURE_INFO = "en-US";
        public const string DATE_FORMAT          = "MMM dd, yyyy";
    }
}
