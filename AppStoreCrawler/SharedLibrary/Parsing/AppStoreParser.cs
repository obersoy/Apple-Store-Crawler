using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
