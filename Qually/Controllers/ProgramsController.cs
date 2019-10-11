using Microsoft.AspNetCore.Mvc;

namespace Qually.Controllers
{
    public class ProgramsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BotQually()
        {
            return View();
        }

        public IActionResult QuallyFlash()
        {
            return View();
        }

        public IActionResult AccountHolder()
        {
            return View();
        }

        public IActionResult Scripts()
        {
            return View();
        }

        public IActionResult BQGuide()
        {
            return View();
        }

        public IActionResult QFGuide()
        {
            return View();
        }

        public IActionResult AHGuide()
        {
            return View();
        }

        public IActionResult ScriptsGuide()
        {
            return View();
        }
    }
}
