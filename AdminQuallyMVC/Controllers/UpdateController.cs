using Microsoft.AspNetCore.Mvc;
using QuallyLib;
using AdminQuallyMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class UpdateController : Controller
    {
        private readonly QuallyContext db;
        private readonly IHostingEnvironment environment;

        public UpdateController(QuallyContext context, IHostingEnvironment environment)
        {
            db = context;
            this.environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            UpdateModel model = new UpdateModel
            {
                Updates = await db.Updates.Select(u => new UpdateSafe { Id = u.Id, Program = u.Program, Version = u.Version }).ToListAsync(),
                Programs = await db.Programs.ToListAsync()
            };
            return View(model);
        }

        public IActionResult AddProgram()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProgram(QuallyLib.Program program)
        {
            db.Programs.Add(program);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProgram(int? id)
        {
            if (id != null)
            {
                var program = await db.Programs.FindAsync(id);
                if (program != null)
                {
                    db.Programs.Remove(program);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти программу!" });
        }

        public IActionResult AddUpdate()
        {
            ViewBag.ListPrograms = new SelectList(db.Programs.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdate(Update update, IFormFile zip)
        {
            if (zip != null)
            {
                var program = await db.Programs.FindAsync(update.ProgramId);
                if (Version.Parse(update.Version) > Version.Parse(program.Version))
                {
                    program.Version = update.Version;
                }
                byte[] zipData = null;
                using (var binaryReader = new BinaryReader(zip.OpenReadStream()))
                {
                    zipData = binaryReader.ReadBytes((int)zip.Length);
                }
                update.Zip = zipData;
                db.Updates.Add(update);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Error", new { message = $"Произошла ошибка при загрузке файла {zip.FileName}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Download(int? id)
        {
            var update = await db.Updates.FindAsync(id);
            if (update != null)
            {
                return File(update.Zip, "archive/zip", $"{update.Program}_{update.Version}.zip");
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти запрашиваемое обновление!" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeVersion(int? id, string version)
        {
            if (id != null)
            {
                var program = await db.Programs.FindAsync(id);
                if (program != null)
                {
                    program.Version = version;
                    db.Programs.Update(program);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти программу!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var update = await db.Updates.FindAsync(id);
                if (update != null)
                {
                    db.Updates.Remove(update);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = "Неверный id!" });
        }
    }
}
