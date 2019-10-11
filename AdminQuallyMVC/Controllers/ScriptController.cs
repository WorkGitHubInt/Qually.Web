using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class ScriptController : Controller
    {
        private readonly QuallyContext db;

        public ScriptController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Scripts.ToListAsync());
        }

        public IActionResult Add()
        {
            Script script = new Script
            {
                Title = "Новый скрипт",
                Content = "",
                Url = "https://botqually.ru",
                Version = "1.0"
            };
            return View(script);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Script script)
        {
            db.Scripts.Add(script);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task <IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Script script = await db.Scripts.FindAsync(id);
                if (script != null)
                {
                    return View(script);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Script script)
        {
            db.Scripts.Update(script);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id != null)
            {
                Script script = await db.Scripts.FindAsync(id);
                if (script != null)
                {
                    db.Scripts.Remove(script);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
    }
}
