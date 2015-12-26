using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using FGapp.Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Runtime.Caching;

namespace FGapp.Controllers
{
    //[RoutePrefix("site")]
    public class SiteController : Controller
    {
        // GET: Site
        public async Task<ActionResult> Index(string state, string owner, string q = "", string sort = null)
        {

            string[] str = null;
            if (!string.IsNullOrEmpty(state))
            {
                str = state.Split(',');
                for (int i = 0; i < str.Length; i++) { str[i] = Helper.StateSearchConvertor(str[i]); }
            }
            else
                str = Helper.Jurisdictions;

            string[] owns = null;
            if (!string.IsNullOrEmpty(owner))
            {
                owns = owner.Split(',');
                
            }
       
            dynamic a = null;

            a = await SearchService.SearchAsync(!string.IsNullOrEmpty(q) ? q : "", str, owns, 1000, sort);       //jur search   

            ViewBag.searchString = q;
            ViewBag.state = string.Join(",", str);
            @ViewBag.owner = owns!=null? string.Join(",", owns):"";
            ViewBag.sort = sort;
            //ViewBag.locationJur = jur;
            ViewBag.locationJson = JsonConvert.SerializeObject(a);
            //return Json(s, JsonRequestBehavior.AllowGet);
            return View(a);//return Content( state + " home - list all sites");

        }


        //[Route("site/{key:guid}")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //            return Content("show site: "+ key);

            var site = (Site)DocumentDBRepository<Site>.GetItem(d => d.Id == id);
            ViewBag.siteName = site.Name;
            ViewBag.Lat = site.geometry.coordinates[1];
            ViewBag.Lng = site.geometry.coordinates[0];
            ViewBag.Suburb = site.SiteSuburb;
            
            var prices = (SiteProduct)DocumentDBRepository<SiteProduct>.GetItems(d => d.SiteId == id).FirstOrDefault();
            if (prices == null)
            {
                var p = new SiteProduct { SiteId = id, SiteName = site.Name, State = site.State, CreatedDate = DateTime.Now };
                await DocumentDBRepository<SiteProduct>.CreateItemAsync(p); //create doco}
                return View(p);
            }
            //ViewData["Prices"]= prices;

            return View(prices);
        }

        //
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DocDBSite item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<DocDBSite>.CreateItemAsync(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }


        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DocDBSite item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<DocDBSite>.UpdateItemAsync(item.id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = (DocDBSite)DocumentDBRepository<DocDBSite>.GetItem(d => d.id == id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Bulk(string lat, string lng)
        {
            var a = await SearchService.SearchAsync(null, lat, lng, 2, 10, "Name") as AS_SearchResponse;
            if (a.Results != null)
            {
                return View(a.Results);
            }

            return View();
            //            return Content("bulk here..." + lat + "," + lng);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Bulk(string lat, string lng, string bulkfueltype, string bulkPrice)//string BioE10Unleaded, string Unleaded91, string PremiumUnleaded95, string PremiumUnleaded98, string Diesel, string LPG)
        {
            //return Content(lat + "==>" + bulkfueltype + ", " + bulkPrice); //", " + PremiumUnleaded95 + ", " + PremiumUnleaded98 + ", " + Diesel + ", " + LPG);
            double cost = 0.0;
            if (!Helper.CheckFuelType(bulkfueltype) || !double.TryParse(bulkPrice, out cost))
            {
                ViewBag.ResultMessage = "Error! Please verify your inputs!";
                return View();
            }

            var a = await SearchService.SearchAsync(null, lat, lng, 2, 10, "Name") as AS_SearchResponse;
            if (a.Results != null)
            {
                foreach (var item in a.Results)
                {
                    AddPrice(item.Id, bulkfueltype, bulkPrice); //TODO: update this for other products
                }
                ViewBag.ResultMessage = "Bulk insert successfully !";
                //return RedirectToAction("Bulk", "Site", new { lat= lat, lng = lng});
            }
            return View();
        }


        [Route("site/search/{key}")]
        public async Task<ActionResult> APISiteSearch(string key)
        {
            var count = Request.QueryString["count"];
            var maxCount = 100;
            var rq = Request.QueryString["filter"];
            string[] str = null;
            if (!string.IsNullOrEmpty(rq))
            {
                str = rq.Split(',');
                for (int i = 0; i < str.Length; i++) { str[i] = Helper.StateSearchConvertor(str[i]); }
            }
            else
                str = Helper.Jurisdictions;

            if (!string.IsNullOrEmpty(count))
                maxCount = int.Parse(count);
            else
                maxCount = 10;

            //var c = filter.Length;
            var a = await SearchService.SearchAsync(key, str, null, maxCount, "");


            var s = JsonConvert.SerializeObject(a);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        [Route("site/search/location/{la}/{ln}")]
        public async Task<ActionResult> APISiteSearch(string la, string ln)
        {
            var distance = Request.QueryString["distance"];
            var maxDistance = 25;
            if (!string.IsNullOrEmpty(distance))
                maxDistance = int.Parse(distance);

            var a = await SearchService.SearchAsync(null, la, ln, maxDistance, 50, "Name");

            //why can't convert 
            var s = JsonConvert.SerializeObject(a);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        [Route("site/price/add/{siteId:guid}/{type:regex(E10|ULP|DSL|PULP95|PULP98|LPG)}/{price}")]
        public async Task<ActionResult> AddPrice(string siteId, string type, string price)
        {
            //return Content("hello " + siteId);
            if (string.IsNullOrEmpty(siteId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = (Site)DocumentDBRepository<Site>.GetItem(d => d.Id == siteId);

            if (item == null)
            {
                return HttpNotFound("no site");
            }

            double p;
            bool isNumeric = double.TryParse(price, out p);
            if (!isNumeric)
                return HttpNotFound();

            var upd = await DocumentDBRepository<DocDBSite>.ExecuteStoredProcedureUpdatePriceAsync(siteId, type, p, "API-Insert");
            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        [Route("site/weather/{suburb}")]
        public async Task<ActionResult> APIWeather(string suburb)
        {
            var weather = await Helper.GetWeather("", suburb);
            return Json(weather, JsonRequestBehavior.AllowGet);

        }
    }
}