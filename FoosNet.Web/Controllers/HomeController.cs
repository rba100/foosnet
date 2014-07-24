using System.Web.Mvc;

namespace FoosNet.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return Content("This is the FoosNet web server.");
        }
    }
}
