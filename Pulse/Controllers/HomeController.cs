using System.Web.Mvc;
using Pulse.Services;

namespace Pulse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISettingsService _settingsService;

        public HomeController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ActionResult Index()
        {
            ViewBag.GoogleMapsUrl = _settingsService.GoogleMapsUrl;
            return View();
        }

    }
}