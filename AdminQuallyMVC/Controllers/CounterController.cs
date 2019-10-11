using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class CounterController : Controller
    {
        private readonly QuallyContext db;

        public CounterController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View(db.Counters.Include(m => m.Program).ToList().OrderByDescending(t => t.Date.Month).ThenByDescending(t => t.Date.Day).ThenByDescending(t => t.Date.Hour).ThenByDescending(t => t.Date.Minute).ThenByDescending(t => t.Date.Second));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var counter = await db.Counters.FindAsync(id);
                if (counter != null)
                {
                    db.Counters.Remove(counter);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти {id}!" });
        }
    }
}
