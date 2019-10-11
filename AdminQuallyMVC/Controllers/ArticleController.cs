using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;


namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly QuallyContext db;

        public ArticleController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Articles.ToListAsync());
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Article article)
        {
            db.Articles.Add(article);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var article = await db.Articles.FindAsync(id);
                if (article != null)
                {
                    return View(article);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти новость {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article article)
        {
            db.Articles.Update(article);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var article = await db.Articles.FindAsync(id);
                if (article != null)
                {
                    db.Articles.Remove(article);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти новость {id}!" });
        }
    }
}
