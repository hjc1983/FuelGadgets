using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using FGapp.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace FGapp.Controllers
{
    public class PriceController : Controller
    {
        // GET: Price
        public ActionResult Index()
        {
            var items = DocumentDBRepository<SiteProduct>.GetItems(d => d.SiteId != string.Empty);
            return View(items);
        }

        //
        [Authorize]
        [Route("price/create/{type:regex(E10|ULP|DSL|PULP95|PULP98|LPG)}/{key:guid}")]
        public async Task<ActionResult> Create(string type ,string key)
        {

            if (Helper.VerifyFuelType(type))
            {
                var site = (DocDBSite)DocumentDBRepository<DocDBSite>.GetItem(x => x.id == key);
                if(site!=null)
                {
                    ViewBag.FuelType = type;
                    var item = (SiteProduct)DocumentDBRepository<SiteProduct>.GetItem(x => x.SiteId == key);
                    if (item != null)
                    {
                        item.FuelType = type.ToUpper();
                        return View(item);
                    }
                    else
                    {
                        var p = new SiteProduct { SiteId = key, SiteName = site.Name, FuelType = type.ToUpper(), CreatedDate = DateTime.Now };
                        await DocumentDBRepository<SiteProduct>.CreateItemAsync(p); //create doco}
                        return View(p);
                    }
                }
            }
            //return Content("create " +key);
            ViewBag.Message = "Error can't locate you request!";
            return RedirectToAction("Index");
            //return View("Index");
        }

        [HttpPost]
        [Authorize]
        [Route("price/create/{type:regex(E10|ULP|DSL|PULP95|PULP98|LPG)}/{key:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SiteProduct p) //Create([Bind(Include = "Id,SiteId,FuelType,Cost,SubmittedBy,IsPublished")] Price p)
        {
            if (ModelState.IsValid)
            {
                var site = (DocDBSite)DocumentDBRepository<DocDBSite>.GetItem(x => x.id == p.SiteId);
                if(site != null) {
                    DocumentDBRepository<DocDBSite>.ExecuteStoredProcedureUpdatePriceAsync(p.SiteId, p.FuelType, p.FuelValue, User.Identity.GetUserId());
                    return RedirectToAction("Details", "Site", new { id = p.SiteId });
                }
                return RedirectToAction("Index", "Site");

            }
            return View(p);
        }

        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SiteId,FuelType,Cost,SubmittedBy,IsPublished")] Price p)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Price>.UpdateItemAsync(p.Id, p);
                return RedirectToAction("Index");
            }

            return View(p);
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Price item = (Price)DocumentDBRepository<Price>.GetItem(d => d.Id == id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        //
        //[HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var doc = await DocumentDBRepository<SiteProduct>.DeleteDocument(id);

            if (doc == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index");
        }
    }
}