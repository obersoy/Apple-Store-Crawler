using HtmlAgilityPack;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SharedLibrary.Parsing
{
    public class AppStoreParser
    {
        public IEnumerable<String> ParseCategoryUrls (string rootHtmlPage)
        {
            // Creating Html Map, and loading root page html on it
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (rootHtmlPage);

            // Reaching Nodes of Interest
            foreach (var htmlNode in map.DocumentNode.SelectNodes (Consts.XPATH_CATEGORIES_URLS))
            {
                // Checking for the Href Attribute
                HtmlAttribute href = htmlNode.Attributes["href"];

                // Sanity Check
                if (href != null)
                {
                    yield return href.Value;
                }
            }
        }

        public IEnumerable<String> ParseCharacterUrls (string htmlResponse)
        {
            // Creating HTML Map based on the html response
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (htmlResponse);

            // Reaching nodes of interest
            foreach (HtmlNode characterNode in map.DocumentNode.SelectNodes (Consts.XPATH_CHARACTERS_URLS))
            {
                // Checking for Href Attribute within the node
                HtmlAttribute href = characterNode.Attributes["href"];

                // Sanity Check
                if (href != null)
                {
                    yield return href.Value;
                }
            }
        }

        public IEnumerable<String> ParseNumericUrls (string htmlResponse)
        {
            // Creating HTML Map based on the html response
            HtmlDocument map = new HtmlDocument();
            map.LoadHtml(htmlResponse);

            // Reaching nodes of interest
            foreach (HtmlNode characterNode in map.DocumentNode.SelectNodes(Consts.XPATH_NUMERIC_URLS))
            {
                // Checking for Href Attribute within the node
                HtmlAttribute href = characterNode.Attributes["href"];

                // Sanity Check
                if (href != null)
                {
                    yield return href.Value;
                }
            }
        }

        public bool IsLastPage (string htmlResponse)
        {
            // Creating HTML Map based on the html response
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (htmlResponse);

            // Trying to reach "Next" node
            return map.DocumentNode.SelectSingleNode (Consts.XPATH_NEXT_PAGE) == null;
        }
        
        public String ParseLastPageUrl (string htmlResponse)
        {
            // Creating HTML Map based on the html response
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (htmlResponse);

            return map.DocumentNode.SelectNodes (Consts.XPATH_LAST_PAGE).Last().Attributes["href"].Value;
        }

        public IEnumerable<String> ParseAppsUrls (string htmlResponse)
        {
            // Creating HTML Map based on the html response
            HtmlDocument map = new HtmlDocument();
            map.LoadHtml(htmlResponse);

            // Reaching nodes of interest
            foreach (HtmlNode characterNode in map.DocumentNode.SelectNodes (Consts.XPATH_APPS_URLS))
            {
                // Checking for Href Attribute within the node
                HtmlAttribute href = characterNode.Attributes["href"];

                // Sanity Check
                if (href != null)
                {
                    yield return href.Value;
                }
            }
        }

        public AppleStoreAppModel ParseAppPage (string htmlResponse)
        {
            // Creating HTML Map based on the html response
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (htmlResponse);

            // Instantiating Empty Parsed App
            AppleStoreAppModel parsedApp = new AppleStoreAppModel ();

            // Reaching nodes of interest
            parsedApp.name          = GetNodeValue (map, Consts.XPATH_TITLE).Trim();
            parsedApp.developerName = GetAppDeveloperName (map);
            parsedApp.developerUrl  = GetDeveloperUrl (map);
            parsedApp.price         = GetAppPrice (map);
            parsedApp.isFree        = parsedApp.price == 0.0 ? true : false;
            parsedApp.category      = GetAppCategory (map);
            parsedApp.updateDate    = GetAppUpdateDate (map);
            //parsedApp.description = GetAppDescription (map); //TODO: Figure out how to get description on a separated request
            parsedApp.version       = GetAppVersion (map);
            parsedApp.size          = GetAppSize (map);
            parsedApp.thumbnailUrl  = GetThumbnailUrl (map);
            parsedApp.languages     = GetLanguages (map);


            return parsedApp;
        }

        private string GetAppDeveloperName (HtmlDocument map)
        {
            string developerName = GetNodeValue (map, Consts.XPATH_DEVELOPER_NAME);

            return String.IsNullOrEmpty (developerName) ? String.Empty : developerName.Replace ("By", String.Empty).Trim().ToUpper();
        }
        private string GetDeveloperUrl (HtmlDocument map)
        {
            // Developer Url Node
            HtmlNode devUrlNode = map.DocumentNode.SelectSingleNode (Consts.XPATH_DEVELOPER_URL);

            // Returning Url
            return devUrlNode.Attributes["href"].Value;
        }

        private double GetAppPrice (HtmlDocument map)
        {
            // Replacing App Price "dot" decimal separator with a comma to allow correct double conversion
            string stringPrice = GetNodeValue (map, Consts.XPATH_APP_PRICE).Replace ('.',',');

            // Checking for "free" app
            if (stringPrice.IndexOf ("free", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return 0;
            }

            // Else, parses the correct price out of the node
            // Culture info is used to determine the correct currency symbol to be removed. This might change depending on
            // the store country or your own IP sometimes
            CultureInfo cInfo = CultureInfo.GetCultureInfo (Consts.CURRENT_CULTURE_INFO);
            return Convert.ToDouble (stringPrice.Replace (cInfo.NumberFormat.CurrencySymbol, String.Empty));
        }

        private string GetAppCategory (HtmlDocument map)
        {
            // Reaching Category Node
            return GetNodeValue (map, Consts.XPATH_CATEGORY).Trim();
        }

        private DateTime GetAppUpdateDate (HtmlDocument map)
        {
            // Reaching Node that contains the update date
            HtmlNode updateDateNode = map.DocumentNode.SelectSingleNode (Consts.XPATH_UPDATE_DATE);
            updateDateNode = updateDateNode.FirstChild.NextSibling;

            // Date Text
            string dateTxt = updateDateNode.InnerText;

            // Parsing Out 
            return DateTime.ParseExact (dateTxt, Consts.DATE_FORMAT, new CultureInfo (Consts.CURRENT_CULTURE_INFO));
        }

        private string GetAppDescription (HtmlDocument map)
        {
            return GetNodeValue (map, Consts.XPATH_DESCRIPTION);
        }

        private string GetAppVersion (HtmlDocument map)
        {
            // Reaching Node that contains the Version number
            HtmlNode versionNode = map.DocumentNode.SelectSingleNode (Consts.XPATH_VERSION);
            versionNode = versionNode.ParentNode.FirstChild.NextSibling;

            return versionNode.InnerText.Trim();
        }

        private string GetAppSize (HtmlDocument map)
        {
            // Reaching Node that contains the Version number
            HtmlNode versionNode = map.DocumentNode.SelectSingleNode (Consts.XPATH_APP_SIZE);
            versionNode = versionNode.ParentNode.FirstChild.NextSibling;

            return versionNode.InnerText.Trim ();
        }

        private string GetThumbnailUrl (HtmlDocument map)
        {
             // Reaching Thumbnail node
            HtmlNode thumbnailNode = map.DocumentNode.SelectSingleNode (Consts.XPATH_THUMBNAIL);

            // Reaching Src attribute of the node
            return thumbnailNode.Attributes["src-swap-high-dpi"].Value;
        }

        private string[] GetLanguages (HtmlDocument map)
        {
            // Reaching Node that contains the Version number
            HtmlNode languagesNode = map.DocumentNode.SelectSingleNode (Consts.XPATH_LANGUAGES);
            languagesNode          = languagesNode.FirstChild.NextSibling;

            return languagesNode.InnerText.Split (',').Select (t => t.Trim()).ToArray();
        }

        private string GetNodeValue (HtmlDocument map, string xPath)
        {
            var node = map.DocumentNode.SelectSingleNode (xPath);

            return node == null ? String.Empty : HttpUtility.HtmlDecode (node.InnerText);
        }
    }
}
