using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using QuallyLib;
using AdminQuallyMVC.ViewModels.Orders;
using AdminQuallyMVC.ViewModels;
using System;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly QuallyContext db;

        public OrderController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index(string pcid, int page = 1)
        {
            IQueryable<Order> orders = db.Orders.AsNoTracking();
            //фильтрация
            if (!string.IsNullOrEmpty(pcid))
            {
                orders = orders.Where(o => o.UserId.Contains(pcid));
            }
            //пагинация
            int pageSize = 30;
            var count = await orders.CountAsync();
            var items = await orders.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            OrderViewModel viewModel = new OrderViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                FilterViewModel = new FilterViewModel(pcid),
                Orders = items
            };
            return View(viewModel);
        }

        public IActionResult Add()
        {
            Order order = new Order
            {
                UserId = (db.Users.Last().Id + 1).ToString(),
                Duration = 1,
                Lvl = SubscriptionLvl.Level_1,
                Sum = 100,
                Paid = false,
                Description = "Тестовая подписка",
            };
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Order order)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == order.UserId);
            if (user != null)
            {
                order.Id = Guid.NewGuid().ToString();
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {order.UserId}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Orders");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FindAsync(id);
                if (order != null)
                {
                    return View(order);
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти заказ {id}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Order order)
        {
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FindAsync(id);
                if (order != null)
                {
                    db.Orders.Remove(order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти заказ {id}!" });
        }
    }
}
