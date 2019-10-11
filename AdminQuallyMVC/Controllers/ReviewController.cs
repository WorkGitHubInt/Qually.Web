using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly QuallyContext db;

        public ReviewController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View(db.Reviews.ToList());
        }

        public async  Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var review = await db.Reviews.FindAsync(id);
                if (review != null)
                {
                    return View(review);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти отзыв {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var review = await db.Reviews.FindAsync(id);
                if (review != null)
                {
                    db.Reviews.Remove(review);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти отзыв {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int? id)
        {
            if (id != null)
            {
                var review = await db.Reviews.FindAsync(id);
                if (review != null)
                {
                    review.Approved = true;
                    db.Reviews.Update(review);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти отзыв {id}!" });
        }
    }
}
