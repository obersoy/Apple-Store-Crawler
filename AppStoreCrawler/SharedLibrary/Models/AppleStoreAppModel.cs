using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class AppleStoreAppModel
    {
        public ObjectId _id                      { get; set; }
        public string   name                     { get; set; }
        public string   developerName            { get; set; }
        public string   developerUrl             { get; set; }
        public double   price                    { get; set; }
        public bool     isFree                   { get; set; }
        public string   description              { get; set; }
        public string   thumbnailUrl             { get; set; }
        public string   compatibility            { get; set; }
        public string   category                 { get; set; }
        public DateTime updateDate               { get; set; }
        public string   version                  { get; set; }
        public string   size                     { get; set; }
        public string[] languages                { get; set; }
        public int      minimumAge               { get; set; }
        public string[] ageRatingReasons         { get; set; }
        public Rating   rating                   { get; set; }
        public InAppPurchase[] topInAppPurchases { get; set; }

        public AppleStoreAppModel ()
        {
            _id = new ObjectId ();
        }

        public string ToJson ()
        {
            return JsonConvert.SerializeObject (this);
        }
    }

    public class Rating
    {
        public int starsRatingCurrentVersion { get; set; }
        public int starsVersionAllVersions   { get; set; }
        public int ratingsCurrentVersion     { get; set; }
        public int ratingsAllVersions        { get; set; }

        public Rating ()
        {
            starsRatingCurrentVersion = 0;
            starsVersionAllVersions   = 0;
            ratingsCurrentVersion     = 0;
            ratingsAllVersions        = 0;
        }
    }

    public class InAppPurchase
    {
        public int ranking       { get; set; }
        public string inAppName  { get; set; }
        public double inAppPrice { get; set; }
    }
}
