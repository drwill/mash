using Mash.AppSettings;
using SampleWebApp.Models;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var settings = Settings.Instance;

            return View(SettingsHelper.GetPropertyValues(settings));
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
    }
}