using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;
using System.Linq;
using System.Threading.Tasks;

namespace AdminQuallyMVC
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly QuallyContext db;

        public MessageController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View(db.Messages.Include(m => m.Program).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string status)
        {
            var message = db.Messages.Find(id);
            message.Status = status;
            db.Messages.Update(message);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var message = await db.Messages.FindAsync(id);
                if (message != null)
                {
                    db.Messages.Remove(message);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти сообщение {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Messages");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var message = await db.Messages.Include(m => m.Program).FirstOrDefaultAsync(m => m.Id == id);
                if (message != null)
                {
                    return View(message);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти сообщение {id}!" });
        }
    }
}
