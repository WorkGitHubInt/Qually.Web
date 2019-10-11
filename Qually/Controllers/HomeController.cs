using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qually.Models;

namespace Qually.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        public HomeController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contacts()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Download()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DownloadPost()
        {
            // Путь к файлу
            string file_path = Path.Combine(_appEnvironment.WebRootPath, "launcher", "QuallyLauncher.exe");
            // Тип файла - content-type
            string file_type = "application/exe";
            // Имя файла - необязательно
            string file_name = "QuallyLauncherInstaller.exe";
            return PhysicalFile(file_path, file_type, file_name);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Error2(string message)
        {
            ViewData["Message"] = message;
            return View();
        }
    }
}
