using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using QuallyLib;
using AdminQuallyMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class BuyController : Controller
    {
        private readonly QuallyContext db;

        public BuyController(QuallyContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            SubViewModel subView = new SubViewModel
            {
                SubModels = await db.SubModels.ToListAsync()
            };
            int i = 1;
            foreach (var sub in subView.SubModels)
            {
                subView.Models.Add(new SelectListItem { Value = i.ToString(), Text = sub.Title });
                i++;
            }
            return View(subView);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Fail()
        {
            return View();
        }

        private readonly string testPass1 = "W5MmTrYFCcK7W1vnb19P";
        private readonly string pass1 = "obZY5q3M62NBwDgB5bBB";

        public async Task<IActionResult> Payment(string userId, int subModelId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == userId);
            var subModel = await db.SubModels.FindAsync(subModelId);
            if (user != null && subModel != null)
            {
                decimal sum = 0;
                if (user.Lvl < subModel.Lvl && (user.SubscriptionExpDate - DateTime.Now).TotalDays > 5)
                {
                    decimal currLvlSum = 0;
                    switch ((int)subModel.Lvl)
                    {
                        case 1:
                            currLvlSum = 100M;
                            break;
                        case 2:
                            currLvlSum = 200M;
                            break;
                        case 3:
                            currLvlSum = 400M;
                            break;
                    }
                    sum = subModel.Sum + (Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (currLvlSum / 30));
                }
                else
                {
                    sum = subModel.Sum;
                }
                var order = await db.Orders.FirstOrDefaultAsync(o => o.UserId == userId && !o.Paid);
                if (order == null)
                {
                    order = new Order
                    {
                        UserId = userId,
                        Date = DateTime.Now,
                        Duration = subModel.Duration,
                        Lvl = subModel.Lvl,
                        Sum = sum,
                        Description = subModel.Title
                    };
                    db.Orders.Add(order);
                }
                else
                {
                    order.Duration = subModel.Duration;
                    order.Date = DateTime.Now;
                    order.Lvl = subModel.Lvl;
                    order.Sum = sum;
                    order.Description = subModel.Title;
                    db.Entry(order).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                BuyModel model = new BuyModel
                {
                    Order = order,
                    SingnatureValue = CreateMD5($"qually:{order.Sum}:0:{testPass1}:shp_order={order.Id}:shp_userid={order.UserId}"),
                };
                return View(model);
            }
            return NotFound();
        }

        public async Task<IActionResult> Confirm(string userId, int subModelId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == userId);
            var subModel = await db.SubModels.FindAsync(subModelId);
            if (user != null && subModel != null)
            {
                if (user.Lvl < subModel.Lvl && (user.SubscriptionExpDate - DateTime.Now).TotalDays > 5)
                {
                    decimal sum = 0;
                    decimal currLvlSum = 0;
                    switch ((int)subModel.Lvl)
                    {
                        case 1:
                            currLvlSum = 100M;
                            break;
                        case 2:
                            currLvlSum = 200M;
                            break;
                        case 3:
                            currLvlSum = 400M;
                            break;
                    }
                    sum = subModel.Sum + (Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (currLvlSum / 30));
                    ConfirmModel cm = new ConfirmModel
                    {
                        TotalSum = sum,
                        DifSum = Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (currLvlSum / 30),
                        Days = Convert.ToInt32((user.SubscriptionExpDate - DateTime.Now).TotalDays),
                        UserId = userId,
                        SubModelId = subModelId,
                        UserLvl = user.Lvl,
                        PayLvl = subModel.Lvl
                    };
                    return View(cm);
                }
                else
                {
                    return RedirectToAction("Payment", new { userId = userId, subModelId = subModelId });
                }
            }
            return NotFound();
        }

        public string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
