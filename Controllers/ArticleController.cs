using FGapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace FGapp.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {

            var items = DocumentDBRepository<Article>.GetItems(a => a.FeatureType == "Article").OrderByDescending(x=>x.PublishedDate).Take(6);
            return View(items);
        }

        // GET: Article/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Article/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Create(Article collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await DocumentDBRepository<Article>.CreateItemAsync(collection);
                    return RedirectToAction("Index");
                }
                return View();

            }
            catch
            {
                return View();
            }
        }

        // GET: Article/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            //var prices = (SiteProduct)DocumentDBRepository<SiteProduct>.GetItem(d => d.SiteId == id);

            var items = DocumentDBRepository<Article>.GetItems(a => a.Id == id).FirstOrDefault();
            return View(items);
        }

        // POST: Article/Edit/5
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, Article collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await DocumentDBRepository<Article>.UpdateItemAsync(id, collection);
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Article/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Article/Delete/5
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
