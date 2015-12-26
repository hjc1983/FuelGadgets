using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Collections;

namespace FGapp.Models
{

    public class DocDBSite
    {
        public string Name { get; set; }
        public string description { get; set; }
        public object timestamp { get; set; }
        public object begin { get; set; }
        public object end { get; set; }
        public object altitudeMode { get; set; }
        public int tessellate { get; set; }
        public int extrude { get; set; }
        public int visibility { get; set; }
        public object drawOrder { get; set; }
        public object icon { get; set; }
        public string FEATURETYPE { get; set; }
        public object NAME2 { get; set; }
        public string OWNER { get; set; }
        public object STATUS { get; set; }
        public string SITEADDRESS { get; set; }
        public string SITESUBURB { get; set; }
        public string STATE { get; set; }
        public object POSTCODE { get; set; }
        public string INDUSTRYID { get; set; }
        public string FEATURESOURCE { get; set; }
        public string FEATURERELIABILITY { get; set; }
        public string ATTRIBUTESOURCE { get; set; }
        public string ATTRIBUTERELIABILITY { get; set; }
        public string PLANIMETRICACCURACY { get; set; }
        public string SPATIALACCURACY { get; set; }
        public object METADATACOMMENT { get; set; }
        public string REVISED { get; set; }
        public double E10 { get; set; }
        public int E10Epoch { get; set; }
        public double ULP { get; set; }
        public int ULPEpoch { get; set; }
        public double PULP95 { get; set; }
        public int PULP95Epoch { get; set; }
        public double PULP98 { get; set; }
        public int PULP98Epoch { get; set; }
        public double DSL { get; set; }
        public int DSLEpoch { get; set; }
        public double LPG { get; set; }
        public int LPGEpoch { get; set; }
        public Geometry geometry { get; set; }
        public string id { get; set; }
    }

    public class Site
    {
        List<Site> items = new List<Site>();


        [JsonProperty("@search.score")]
        public string SearchScore { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "OWNER")]
        public string Owner { get; set; }

        [JsonProperty(PropertyName = "SITEADDRESS")]
        public string SiteAddress { get; set; }

        [JsonProperty(PropertyName = "SITESUBURB")]
        public string SiteSuburb { get; set; }

        [JsonProperty(PropertyName = "STATE")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "POSTCODE")]
        public string Postcode { get; set; }

        [JsonProperty(PropertyName = "BioE10Unleaded")]
        public double BioE10Unleaded { get; set; }

        [JsonProperty(PropertyName = "Unleaded91")]
        public double Unleaded91 { get; set; }

        [JsonProperty(PropertyName = "PremiumUnleaded95")]
        public double PremiumUnleaded95 { get; set; }

        [JsonProperty(PropertyName = "PremiumUnleaded98")]
        public double PremiumUnleaded98 { get; set; }

        [JsonProperty(PropertyName = "Diesel")]
        public double Diesel { get; set; }

        [JsonProperty(PropertyName = "LPG")]
        public double LPG { get; set; }

        [JsonProperty(PropertyName = "geometry")]
        public Geometry geometry { get; set; }
        //public List<double> geometry { get; set; }

        //[JsonProperty(PropertyName = "description")]
        //public string DescriptBion { get; set; }

        //[JsonProperty(PropertyName = "FEATURETYPE")]
        //public string FeatureType { get; set; }



        //[JsonProperty(PropertyName = "visibility")]
        //public bool Published { get; set; }

    }
    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
        //public Crs crs { get; set; }
    }
    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }
    public class Properties
    {
        public string name { get; set; }
    }
}