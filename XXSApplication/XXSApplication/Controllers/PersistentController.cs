using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XXSApplication.Controllers
{
    public class PersistentController : Controller
    {
        // GET: Persistent
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