﻿using Newtonsoft.Json;
using FGapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FGapp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(string state, string q = "", string sort = null)
        {
            return View("IndexNew");
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}