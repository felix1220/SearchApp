using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SearchApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult PuzzleBuilder()
        {
            return View();
        }
        public ActionResult WordGridPuzzle()
        {
            return View();
        }
        public ActionResult DThreeTest()
        {
            return View();
        }
        public ActionResult AntPuzzle()
        {
            return View();
        }
        public ActionResult PuzzleViewer()
        {
            return View();
        }
        public ActionResult NewWordBuilder()
        {
            return View();
        }
<<<<<<< HEAD
        public ActionResult PlotColors()
        {
            return View();
        }
        public ActionResult PasteMapper()
=======
        public ActionResult PastMap()
        {
            return View();
        }
        public ActionResult PlotColors()
>>>>>>> 38f69e1cef27bdcc82df6c214e4b73bdb66f4e43
        {
            return View();
        }
    }
}