using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using AdminQuallyMVC.ViewModels.Users;
using AdminQuallyMVC.ViewModels;
using System;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly QuallyContext db;

        public UserController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index(int? lvl, string pcid, int page = 1, SortState sortOrder = SortState.LvlAsc)
        {
            //фильтрация
            IQueryable<User> users = db.Users.AsNoTracking();
            if (lvl != null)
            {
                users = users.Where(u => (int)u.Lvl == lvl);
            }
            if (!string.IsNullOrEmpty(pcid))
            {
                users = users.Where(u => u.PCID.Contains(pcid));
            }
            //сортировка
            switch (sortOrder)
            {
                case SortState.LvlDesc:
                    users = users.OrderByDescending(u => (int)u.Lvl);
                    break;
                default:
                    users = users.OrderBy(u => (int)u.Lvl);
                    break;
            }
            //пагинация
            int pageSize = 20;
            var count = await users.CountAsync();
            var items = await users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            UserViewModel viewModel = new UserViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(lvl, pcid),
                Users = items
            };
            return View(viewModel);
        }

        public IActionResult Add()
        {
            User user = new User
            {
                PCID = RandomString(4),
                Lvl = SubscriptionLvl.Level_No,
                SubscriptionDate = new DateTime(2019, 1, 1),
                SubscriptionExpDate = new DateTime(2019, 1, 2),
                Active = false,
                TrialUse = false,
                UnlimitedSub = false
            };
            return View(user);
        }

        private string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> Add(User user)
        {
            if (user.SubscriptionDate >= new DateTime(2019, 1, 1) && user.SubscriptionExpDate >= new DateTime(2019, 1, 2))
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Error", new { message = "Неверная дата!" });
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Users");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FindAsync(id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {id}!" });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FindAsync(id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FindAsync(id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FindAsync(id);
                if (user != null)
                {
                    user.Active = user.Active ? false : true;
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {id}!" });
        }
    }
}
