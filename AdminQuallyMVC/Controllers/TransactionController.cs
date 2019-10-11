using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using QuallyLib;
using AdminQuallyMVC.ViewModels.Transactions;
using AdminQuallyMVC.ViewModels;
using System;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly QuallyContext db;

        public TransactionController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index(string pcid, int page = 1)
        {
            IQueryable<Transaction> transactions = db.Transactions.AsNoTracking().OrderByDescending(t => t.Date.Month)
                .ThenByDescending(t => t.Date.Day);
            //фильтрация
            if (!string.IsNullOrEmpty(pcid))
            {
                transactions = transactions.Where(t => t.UserId.Contains(pcid));
            }
            //пагинация
            int pageSize = 20;
            var count = await transactions.CountAsync();
            var items = await transactions.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            TransactionViewModel viewModel = new TransactionViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                FilterViewModel = new FilterViewModel(pcid),
                Transactions = items
            };
            return View(viewModel);
        }

        public IActionResult Add()
        {
            Transaction transaction = new Transaction
            {
                Operation_Id = RandomString(4),
                Date = new DateTime(2019, 1, 1),
                Sum = 100,
                UserId = (db.Users.Last().Id + 1).ToString(),
                Email = "test@mail.ru"
            };
            return View(transaction);
        }

        private string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Transaction transaction)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == transaction.UserId);
            if (user != null)
            {
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {transaction.UserId}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Transactions");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id != null)
            {
                Transaction transaction = await db.Transactions.FindAsync(id);
                if (transaction != null)
                {
                    return View(transaction);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти перевод {id}!" });
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id != null)
            {
                Transaction transaction = await db.Transactions.FindAsync(id);
                if (transaction != null)
                {
                    return View(transaction);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти перевод {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Transaction transaction)
        {
            db.Transactions.Update(transaction);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                Transaction transaction = await db.Transactions.FindAsync(id);
                if (transaction != null)
                {
                    db.Transactions.Remove(transaction);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти перевод {id}!" });
        }
    }
}
