using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilsLib;

namespace SharedLibrary
{
    public class RequestsHandler
    {
        public string GetRootPage()
        {
            string htmlResponse = String.Empty;
            int currentRetry = 0, maxRetries = 100;

            using (WebRequests client = new WebRequests())
            {
                // (Re) Trying to reach Root Page
                do
                {
                    htmlResponse = client.Get (Consts.ROOT_STORE_URL);

                    currentRetry++;
                
                } while (String.IsNullOrEmpty (htmlResponse) && currentRetry <= maxRetries);
            }

            return htmlResponse;
        }

        public string Get (string url)
        {
            using (WebRequests httpClient = new WebRequests ())
            {
                httpClient.UserAgent = Consts.USER_AGENT;
                string htmlResponse  = httpClient.Get (url);

                return htmlResponse;
            }
        }
    }
}
