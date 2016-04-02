using System.Web.Mvc;

namespace Dispatcher.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Dispatcher";

            return View();
        }
    }
}
