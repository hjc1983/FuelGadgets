using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Configuration;

namespace FGapp.Models
{
    public class SearchService
    {
        private static string EscapeODataString(string s)
        {
            return Uri.EscapeDataString(s).Replace("\'", "\'\'");
        }
        public static async Task<dynamic> SearchAsync(string searchString, string lat, string lon, int distance, int? maxItems, string sort)
        {
            var searchParameters = new SearchParameters();

            if (string.IsNullOrEmpty(searchString))
                searchString = "*";

            if (!string.IsNullOrWhiteSpace(lat) && !string.IsNullOrWhiteSpace(lon))
            {
                //if (distance <= 25) distance = 25;

                string filter = "geo.distance(geometry,geography'POINT(" + EscapeODataString(lon) + " " + EscapeODataString(lat) + ")') le " + EscapeODataString(distance.ToString());
                searchParameters.Filter = filter;
            }

            if (maxItems.HasValue)
            {
                searchParameters.Top = maxItems;
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                searchParameters.OrderBy = new string[] { sort };
            }

            searchParameters.IncludeTotalResultCount = true;

            return await DoSearchAsync(searchString, searchParameters);
        }
        public static async Task<dynamic> SearchAsync(string searchString, string[] jurisdiction, string[] owner, int? maxItems, string sort)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                searchString = "*";
            else
                searchString += "*";

            var searchParameters = new SearchParameters();

            searchParameters.SearchFields = new string[] { "OWNER", "Name", "SITESUBURB" };

            if (jurisdiction!=null && jurisdiction.Length > 0)
            {
                var jur = "";
                for (int i = 0; i < jurisdiction.Length; i++)
                {
                    if (!string.IsNullOrEmpty(jurisdiction[i]))
                    {
                        if (i == 0)
                            jur = "(STATE eq '" + jurisdiction[i] + "'";
                        else
                            jur += " or STATE eq '" + jurisdiction[i] + "'"; //EscapeODataString()??
                    }
                }
                jur += ")"; 

                
                searchParameters.Filter = jur;
            }

            if(owner !=null && owner.Length > 0)
            {
                var ops = searchParameters.Filter;
                for (int i = 0; i < owner.Length; i++)
                {
                    if (i == 0)
                        ops += "and (OWNER eq '" + owner[i] + "'";
                    else
                        ops += " or OWNER eq '" + owner[i] + "'"; //EscapeODataString()??
                }
                ops += ")";
                //jur += ") and (OWNER eq 'BP')"; **hack
                searchParameters.Filter = ops;
            }

            if (maxItems.HasValue)
            {
                searchParameters.Top = maxItems;
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                searchParameters.OrderBy = new string[] { sort };
            }
            searchParameters.Facets = new string[] { "OWNER", "STATE", "SITESUBURB,count:10" };
            searchParameters.IncludeTotalResultCount = true;

            return await DoSearchAsync(searchString, searchParameters);
        }

        private static async Task<dynamic> DoSearchAsync(string searchString, SearchParameters parameters)
        {
            var rsp = new AS_SearchResponse();
            var facet = new AS_FacetResults();
            var searchResults = new List<Site>();
            var count = 0;

            using (var indexClient = new SearchIndexClient(ConfigurationManager.AppSettings["ServiceName"], ConfigurationManager.AppSettings["IndexName"], new SearchCredentials(ConfigurationManager.AppSettings["QueryKey"])))
            {
                try {

                    var response = await indexClient.Documents.SearchAsync<dynamic>(searchString, parameters);
                    //var user = JsonConvert.DeserializeObject(response.Results.Select(x=>x.Document));
                    count = (int)response.Count;
                    var facets = response.Facets;
                    var values = response.Results; // response.Content.ReadAsStringAsync().Result
                    
                    foreach (var value in values.Select(x => x.Document))
                    {
                        //https://social.msdn.microsoft.com/Forums/en-US/094e32fc-ebbd-4fc4-aeb8-c9bf4d4c9153/process-search-result-in-c?forum=azuresearch

                        searchResults.Add(new Site
                        {
                            SearchScore = value["@search.score"],
                            Id = value["id"],
                            Name = value["Name"],
                            Owner = value["OWNER"],
                            SiteAddress = value["SITEADDRESS"],
                            SiteSuburb = value["SITESUBURB"],
                            State = value["STATE"],
                            Postcode = value["POSTCODE"],
                            BioE10Unleaded = value["E10"],
                            Unleaded91 = value["ULP"],
                            PremiumUnleaded95 = value["PULP95"],
                            PremiumUnleaded98 = value["PULP98"],
                            Diesel = value["DSL"],
                            LPG = value["LPG"],
                            //http://stackoverflow.com/questions/13565245/convert-newtonsoft-json-linq-jarray-to-a-list-of-specific-object-type
                            geometry = (value["geometry"].ToObject(typeof(Geometry))) //(double)(value["geometry"])["coordinates"][0]
                        });
                    }


                    if (facets != null)
                    {
                        foreach (var value in facets)
                        {

                            List<AS_FacetResult> l = new List<AS_FacetResult>();
                            foreach (var item in value.Value)
                            {
                                var r = new AS_FacetResult { Count = item.Count, Value = item.Value };
                                l.Add(r);
                            }
                            facet.Add(value.Key, l);
                        }
                    }
                    rsp =  new AS_SearchResponse { Count = count, Facets = facet, Results = searchResults };
                }
                catch { }

                return rsp;
            }
        }
    }

    public class AS_SearchResponse
    {
        public SearchContinuationToken ContinuationToken { get; set; }
        //
        // Summary:
        //     Gets the total count of results found by the search operation, or null if the
        //     count was not requested.
        //
        // Remarks:
        //     If present, the count may be greater than the number of results in this response.
        //     In that case, use the ContinuationToken property to fetch the next page of results.
        [JsonProperty("Count")]
        public long? Count { get; set; }
        //
        // Summary:
        //     Gets a value indicating the percentage of the index that was included in the
        //     query, or null if MinimumCoverage was not set in the SearchParameters.
        [JsonProperty("Coverage")]
        public double? Coverage { get; set; }
        //
        // Summary:
        //     Gets the facet query results for the search operation, or null if the query did
        //     not include any facet expressions.
        [JsonProperty("Facets")]
        public AS_FacetResults Facets { get; set; }
        //
        // Summary:
        //     Gets the sequence of results returned by the query.
        [JsonProperty("Results")]
        public IList<Site> Results { get; set; }

    }
    //
    // Summary:
    //     A single bucket of a facet query result that reports the number of documents
    //     with a field falling within a particular range or having a particular value or
    //     interval.
    public class AS_FacetResult
    {
        //
        // Summary:
        //     Initializes a new instance of the Facet class.


        //
        // Summary:
        //     Gets the approximate count of documents falling within the bucket described by
        //     this facet.
        public long Count { get; set; }
        //
        // Summary:
        //     Gets a value indicating the inclusive lower bound of the facet's range, or null
        //     to indicate that there is no lower bound (i.e. -- for the first bucket).
        //public object From { get; set; }
        //
        // Summary:
        //     Gets a value indicating the exclusive upper bound of the facet's range, or null
        //     to indicate that there is no upper bound (i.e. -- for the last bucket).
        //public object To { get; set; }
        //
        // Summary:
        //     Gets a value indicating the type of this facet.
        //public FacetType Type { get; }
        //
        // Summary:
        //     Gets the value of the facet, or the inclusive lower bound if it's an interval
        //     facet.
        public object Value { get; set; }
    }
    public class AS_FacetResults : Dictionary<string, IList<AS_FacetResult>>
    { }

}