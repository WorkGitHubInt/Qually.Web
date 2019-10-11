using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class SubController : Controller
    {
        private readonly QuallyContext db;

        public SubController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.SubModels.ToListAsync());
        }

        public IActionResult Add()
        {
            SubModel sm = new SubModel
            {
                Duration = 1,
                Lvl = SubscriptionLvl.Level_1,
                Sum = 100,
                Title = "Тестовая подписка 1 уровня, 1 месяц"
            };
            return View(sm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SubModel sm)
        {
            db.SubModels.Add(sm);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                SubModel sm = await db.SubModels.FindAsync(id);
                if (sm != null)
                {
                    return View(sm);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти подписку {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SubModel sm)
        {
            db.SubModels.Update(sm);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                SubModel sm = await db.SubModels.FindAsync(id);
                if (sm != null)
                {
                    db.SubModels.Remove(sm);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти подписку {id}!" });
        }
    }
}
