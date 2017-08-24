using System.Web.Mvc;

namespace Pulse.Controllers
{
    public class MotorsController : Controller
    {
        // GET: Motors
        public ActionResult Index()
        {
            return PartialView("Index");
        }
    }
}