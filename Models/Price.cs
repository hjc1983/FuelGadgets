using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FGapp.Models
{

    public class Cost
    {
        [JsonProperty(PropertyName = "total")]
        public double Total { get; set; }

        [JsonProperty(PropertyName = "submittedDate")]
        public DateTime SubmittedDate { get; set; } //user guid...etc

        [JsonProperty(PropertyName = "submittedEpoch")]
        public int SubmittedEpoch { get; set; } //user guid...etc

        [JsonProperty(PropertyName = "submittedBy")]
        public string SubmittedBy { get; set; } //user guid...etc

        [JsonProperty(PropertyName = "isPublished")]
        public bool IsPublished { get; set; }

    }

    public class SiteProduct
    {
        List<Cost> E10_Items = new List<Cost>();
        List<Cost> ULP_Items = new List<Cost>();
        List<Cost>  DSL_Items = new List<Cost>();
        List<Cost> PULP95_Items= new List<Cost>();
        List<Cost>  PULP98_Items = new List<Cost>();
        List<Cost> LPG_Items = new List<Cost>();

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "siteId")]
        public string SiteId { get; set; }

        [JsonProperty(PropertyName = "STATE")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "E10")]
        public List<Cost> BioE10Unleaded
        {
            get { return E10_Items; }
            set { E10_Items = value; }
        }

        [JsonProperty(PropertyName = "ULP")]
        public List<Cost> Unleaded91
        {
            get { return ULP_Items; }
            set { ULP_Items = value; }
        }

        [JsonProperty(PropertyName = "PULP95")]
        public List<Cost> PremiumUnleaded95
        {
            get { return PULP95_Items; }
            set { PULP95_Items = value; }
        }

        [JsonProperty(PropertyName = "PULP98")]
        public List<Cost> PremiumUnleaded98
        {
            get { return PULP98_Items; }
            set { PULP98_Items = value; }
        }

        [JsonProperty(PropertyName = "DSL")]
        public List<Cost> Diesel
        {
            get { return DSL_Items; }
            set { DSL_Items = value; }
        }

        [JsonProperty(PropertyName = "LPG")]
        public List<Cost> Gas
        {
            get { return LPG_Items; }
            set { LPG_Items = value; }
        }

        //public DateEpoch Date { get; set; }

        public string FuelType { get; set; }
        public double FuelValue { get; set; }

        public string SiteName { get; set; }

        [JsonProperty(PropertyName = "isPublished")]
        public bool IsPublished { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; } //user guid...etc

        [JsonProperty(PropertyName = "modifiedDate")]
        public DateTime ModifiedDate { get; set; } //user guid...etc

        [JsonProperty(PropertyName = "FEATURETYPE")]
        public string FeatureType { get { return "Price"; } }
        
    }


    public class Price
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "siteId")]
        public string SiteId { get; set; }

        [JsonProperty(PropertyName = "fuelType")]
        public string FuelType { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public double Cost { get; set; }
        //public DateEpoch Date { get; set; }
        [JsonProperty(PropertyName = "submittedBy")]
        public string SubmittedBy { get; set; } //user guid...etc

        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; } //hidden?

        [JsonProperty(PropertyName = "isPublished")]
        public bool IsPublished { get; set; }

    }

    public class Address
    {
        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "postcode")]
        public string Postcode { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string County { get; set; }

    }
}