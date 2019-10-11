using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminQuallyMVC.Models;
using Microsoft.AspNetCore.Authorization;
using QuallyLib;
using System.Linq;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly QuallyContext db;

        public HomeController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            HomeModel model = new HomeModel
            {
                Admin = User.Identity.Name,
                Users = db.Users.Count(),
                ActiveUsers = db.Users.Where(u => u.Active == true).Count(),
                Orders = db.Orders.Count(),
                Transactions = db.Transactions.Count(),
                Keys = db.Keys.Count(),
                TrialUsers = db.Users.Where(u => u.Trial == true).Count(),
                UnlimitedUsers = db.Users.Where(u => u.UnlimitedSub == true).Count(),
                Messages = db.Messages.Count(),
            };
            decimal sum = 0;
            foreach (var transaction in db.Transactions.ToList())
            {
                sum += transaction.Sum;
            }
            model.Income = sum;
            model.Programs = db.Programs.ToList();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
