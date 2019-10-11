using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;

namespace Qually.Controllers
{
    public class ReviewController : Controller
    {
        private readonly QuallyContext db;

        public ReviewController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ReviewModel reviewModel = new ReviewModel
            {
                Reviews = db.Reviews.ToList()
            };
            return View(reviewModel);
        }

        [HttpPost]
        public async  Task<IActionResult> Add(ReviewModel reviewModel)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == reviewModel.Review.UserID);
            if (user != null && user.Active && !user.Trial)
            {
                if (user.Active)
                {
                    if (!user.Trial)
                    {
                        if (string.IsNullOrEmpty(reviewModel.Review.Name))
                        {
                            reviewModel.Review.Name = "Аноним";
                        }
                        db.Reviews.Add(reviewModel.Review);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    } else
                    {
                        return RedirectToAction("Error2", "Home", new { message = "Пользователь " + reviewModel.Review.UserID + " является пробным!" });
                    }
                } else
                {
                    return RedirectToAction("Error2", "Home", new { message = "Пользователь " + reviewModel.Review.UserID + " не активен!" });
                }
            } else
            {
                return RedirectToAction("Error2", "Home", new { message = "Не найден пользователь " + reviewModel.Review.UserID + "!" });
            }
        }
    }
}
