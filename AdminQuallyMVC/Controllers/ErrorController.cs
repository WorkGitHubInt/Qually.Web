using Microsoft.AspNetCore.Mvc;
using AdminQuallyMVC.Models;

namespace AdminQuallyMVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(string message)
        {
            ErrorModel em = new ErrorModel
            {
                Message = message
            };
            return View(em);
        }
    }
}
