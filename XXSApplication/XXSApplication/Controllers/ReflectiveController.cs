using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XXSApplication.Models;

namespace XXSApplication.Controllers
{
    public class ReflectiveController : Controller
    {
        // GET: Reflective
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Safe()
        {
            return View();
        }
    }
}