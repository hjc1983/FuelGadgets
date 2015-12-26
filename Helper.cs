using FGapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FGapp
{
    public class Helper
    {
        public static string[] Jurisdictions { get { return new string[] { "QUEENSLAND", "NEW SOUTH WALES", "AUSTRALIAN CAPITAL TERRITORY", "VICTORIA", "TASMANIA", "NORTHERN TERRITORY", "SOUTH AUSTRALIA", "WESTERN AUSTRALIA" }; } }
        public static string[] Owners { get { return new string[] { "Caltex", "BP", "7-Eleven Pty Ltd", "Shell", "Independent Fuel Supplies", "United", "Peregrine Corporation", "Ampol", "Matilda Fuels", "Neumann Petroleum" }; } }

        public static string StateSearchConvertor(string shortName)
        {
            //dic have better performance??
            switch (shortName.ToLower())
            {
                case "qld":
                case "queensland":
                    return "QUEENSLAND";
                case "nsw":
                case "new south wales":
                    return "NEW SOUTH WALES";
                case "act":
                case "australian capital territory":
                    return "AUSTRALIAN CAPITAL TERRITORY";
                case "vic":
                case "victoria":
                    return "VICTORIA";
                case "tas":
                case "tasmania":
                    return "TASMANIA";
                case "nt":
                case "northern territory":
                    return "NORTHERN TERRITORY";
                case "sa":
                case "south australia":
                    return "SOUTH AUSTRALIA";
                case "wa":
                case "western australia":
                    return "WESTERN AUSTRALIA";

                default: return "";
            }
        }

        public static bool CheckFuelType(string type)
        {
            switch (type.ToUpperInvariant())
            {
                case "E10":
                case "ULP":
                case "DSL":
                case "PULP95":
                case "PULP98":
                case "LPG":
                    return true;
                default:
                    return false;
            }
        }
        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }


        private static Dictionary<string, string> fuelType = new Dictionary<string, string>() { { "E10", "Unleaded E10" }, { "ULP", "Unleaded 91" }, { "DSL", "Diesel" }, { "PULP95", "Premium 95" }, { "PULP98", "Premium 98" }, { "LPG", "Liquefied petroleum gas" } };
        public static bool VerifyFuelType(string key)
        {
            return fuelType.ContainsKey(key.ToUpper());
        }
        public static string GetFuelValue(string key)
        {
            string value = "";
            if (fuelType.TryGetValue(key.ToUpper(), out value))
                return value;
            else
                return "";
        }


        public static async Task<Weather> GetWeather(string type, string location)
        {
            type = (type != "forecast") ? "current" : "forecast"; //need &days=3 in URL for forecast

            Weather model = null;
            var client = new HttpClient();
            var task = client.GetAsync(string.Format("http://api.apixu.com/v1/{0}.json?key=1c478c6ea9674d749fd33947152207&q={1}", type, location))
              .ContinueWith((taskwithresponse) =>
              {
                  var response = taskwithresponse.Result;
                  var jsonString = response.Content.ReadAsStringAsync();
                  jsonString.Wait();
                  model = JsonConvert.DeserializeObject<Weather>(jsonString.Result);

              });
            task.Wait();
            return model;
        }


        //helper Epoch & Unix Timestamp Conversion - http://www.epochconverter.com/
        public enum Period { Daily, Weekly, Monthly, Quarterly, Yearly };
        public static int GetEpoch(Period period)
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);

            switch (period)
            {
                case Period.Weekly:
                    //Get the last 7 days
                    var day = today.AddDays(-8);
                    return (int)(new DateTime(today.Year, today.Month, day.Day, 0, 0, 0, DateTimeKind.Utc) - new DateTime(1970, 1, 1)).TotalSeconds;
                case Period.Monthly:
                    //Get the previous month's first and last day dates
                    var mfirst = month.AddMonths(-1);
                    var mlast = month.AddDays(-1);
                    return (int)(new DateTime(today.Year, mfirst.Month, 1, 0, 0, 0, DateTimeKind.Utc) - new DateTime(1970, 1, 1)).TotalSeconds;
                case Period.Quarterly:
                    var qfirst = month.AddMonths(-3);
                    return (int)(new DateTime(today.Year, qfirst.Month, 1, 0, 0, 0, DateTimeKind.Utc) - new DateTime(1970, 1, 1)).TotalSeconds;
                case Period.Yearly:
                    var yfirst = today.AddYears(-1);
                    return (int)(new DateTime(yfirst.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc) - new DateTime(1970, 1, 1)).TotalSeconds;
                default:
                    var lday = today.AddDays(-1);
                    return (int)(new DateTime(today.Year, today.Month, lday.Day, 0, 0, 0, DateTimeKind.Utc) - new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }
        public static int GetEpoch()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static string Epoch2string(int epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToShortDateString();
        }
    }
}