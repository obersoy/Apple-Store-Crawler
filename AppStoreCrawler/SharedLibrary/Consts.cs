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
    }
}
