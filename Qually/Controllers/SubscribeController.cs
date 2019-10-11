using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;
using Qually.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using System.Text;
using System.Security.Cryptography;
using Qiwi.BillPayments.Exception;
using System.Net.Http;
using System.Linq;

namespace Qually.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly QuallyContext db;

        public SubscribeController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Fail()
        {
            return View();
        }

        public async Task<IActionResult> Confirm(int? id, string userId)
        {
            ConfirmModel model = new ConfirmModel
            {
                SelectedValue = id,
                UserId = userId
            };
            model.SubModelList = new SelectList(await db.SubModels.ToListAsync(), "Id", "Title", model.SelectedValue.ToString());
            return View(model);
        }

        private readonly string testPass1 = "W5MmTrYFCcK7W1vnb19P";
        private readonly string pass1 = "obZY5q3M62NBwDgB5bBB";

        [HttpPost]
        public async Task<IActionResult> Warning(string userId, int subModelId, OrderType type)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == userId);
            var subModel = await db.SubModels.FindAsync(subModelId);
            if (user != null && subModel != null)
            {
                decimal currLvlSum = 0;
                switch ((int)subModel.Lvl)
                {
                    case 1:
                        currLvlSum = 100M;
                        break;
                    case 2:
                        currLvlSum = 250M;
                        break;
                }
                if (type == OrderType.Upgrade)
                {
                    if (subModel.Lvl > user.Lvl)
                    {
                        return Json(new { redirect = true, url = $"/subscribe/payment?userId={userId}&subModelId={subModelId}&type={type}" });
                    }
                    else
                    {
                        return Json(new { redirect = true, url = $"/Home/Error2?message=У пользователя {userId} уровень больше или равен выбраному!" });
                    }
                }
                if (user.Lvl < subModel.Lvl && (user.SubscriptionExpDate - DateTime.Now).TotalDays > 5)
                {
                    decimal sum = 0;
                    sum = Math.Round(subModel.Sum + (Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (currLvlSum / 30)));
                    WarningModel model = new WarningModel()
                    {
                        TotalSum = sum,
                        DiffSum = Math.Round(Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (currLvlSum / 30)),
                        Days = Convert.ToInt32((user.SubscriptionExpDate - DateTime.Now).TotalDays),
                        SubModel = subModel,
                        UserId = userId,
                        UserLvl = user.Lvl,
                        PayLvl = subModel.Lvl
                    };
                    return PartialView(model);
                }
                else
                {
                    return Json(new { redirect = true, url = $"/subscribe/payment?userId={userId}&subModelId={subModelId}&type={type}" });
                }
            }
            return Json(new { redirect = true, url = $"/Home/Error2?message=Не удалось найти пользователя {userId}" });
        }

        [HttpGet]
        public async Task<IActionResult> Payment(string userId, int subModelId, OrderType type)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == userId);
            var subModel = await db.SubModels.FirstOrDefaultAsync(s => s.Id == subModelId);
            if (user != null && subModel != null)
            {
                decimal currLvlSum = 0;
                switch ((int)subModel.Lvl)
                {
                    case 1:
                        currLvlSum = 100M;
                        break;
                    case 2:
                        currLvlSum = 250M;
                        break;
                }
                PaymentModel model = new PaymentModel();
                decimal sum = 0;
                if (type == OrderType.Upgrade)
                {
                    sum = Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (250M / 30) - Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (100M / 30);
                }
                else
                {
                    if (user.Lvl < subModel.Lvl && (user.SubscriptionExpDate - DateTime.Now).TotalDays > 5)
                    {
                        sum = subModel.Sum + (Convert.ToDecimal((user.SubscriptionExpDate - DateTime.Now).TotalDays) * (currLvlSum / 30));
                    }
                    else
                    {
                        sum = subModel.Sum;
                    }
                }
                var order = await db.Orders.FirstOrDefaultAsync(o => o.UserId == userId && !o.Paid);
                if (order == null)
                {
                    order = new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        Date = DateTime.Now,
                        Duration = subModel.Duration,
                        Lvl = subModel.Lvl,
                        Sum = Math.Round(sum),
                        Description = subModel.Title,
                        Type = type
                    };
                    if (order.Type == OrderType.Upgrade)
                    {
                        order.Description = "Повышение до 2-го уровня";
                    }
                    db.Orders.Add(order);
                }
                else
                {
                    try
                    {
                        var client = BillPaymentsClientFactory.Create(secretKey: "eyJ2ZXJzaW9uIjoiUDJQIiwiZGF0YSI6eyJwYXlpbl9tZXJjaGFudF9zaXRlX3VpZCI6ImRiMzQ2OTIzLTgxNjQtNDA0NC1hNmE2LWMzMDhiODU0NjE0YyIsInVzZXJfaWQiOiI3OTAwNTY4ODQxNyIsInNlY3JldCI6IjdkNWY4NTQ4OGY2ZmE4MzhhMTQ3MjMxZDI5MTRlODg1ZTQ5MTZhMWIzZDFmMzM0MWJlNWI5NTExYzZmNzU5M2IifX0=");
                        client.CancelBill(billId: order.Id);
                    }
                    catch { }
                    db.Orders.Remove(order);
                    await db.SaveChangesAsync();
                    order.Id = Guid.NewGuid().ToString();
                    order.Duration = subModel.Duration;
                    order.Lvl = subModel.Lvl;
                    order.Sum = Math.Round(sum);
                    order.Type = type;
                    if (order.Type == OrderType.Upgrade)
                    {
                        order.Description = "Повышение до 2-го уровня";
                    } else
                    {
                        order.Description = subModel.Title;
                    }
                    db.Orders.Add(order);
                }
                model.SubModel = subModel;
                model.Order = order;
                model.SignatureValue = CreateMD5($"qually:{order.Sum}:0:{pass1}:shp_order={order.Id}:shp_userid={order.UserId}");
                await db.SaveChangesAsync();
                return View(model);
            }
            return RedirectToPage("~/Error2", new { message = "Не удалось найти пользователя!" });
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string orderid, string type)
        {
            var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderid);
            if (order != null)
            {
                switch (type)
                {
                    case "qiwi":
                        var client1 = BillPaymentsClientFactory.Create(secretKey: "eyJ2ZXJzaW9uIjoiUDJQIiwiZGF0YSI6eyJwYXlpbl9tZXJjaGFudF9zaXRlX3VpZCI6ImRiMzQ2OTIzLTgxNjQtNDA0NC1hNmE2LWMzMDhiODU0NjE0YyIsInVzZXJfaWQiOiI3OTAwNTY4ODQxNyIsInNlY3JldCI6IjdkNWY4NTQ4OGY2ZmE4MzhhMTQ3MjMxZDI5MTRlODg1ZTQ5MTZhMWIzZDFmMzM0MWJlNWI5NTExYzZmNzU5M2IifX0=");
                        var billInfo = new CreateBillInfo
                        {
                            BillId = order.Id,
                            Amount = new MoneyAmount
                            {
                                ValueDecimal = order.Sum,
                                CurrencyEnum = CurrencyEnum.Rub
                            },
                            ExpirationDateTime = DateTime.Now.AddDays(15),
                            SuccessUrl = new Uri("https://botqually.ru/subscribe/success")
                        };
                        var response = client1.CreateBill(billInfo);
                        return Redirect(response.PayUrl.ToString());
                    case "yandex":
                        using (var handler = new HttpClientHandler())
                        {
                            handler.AllowAutoRedirect = false;
                            using (var client = new HttpClient(handler))
                            {
                                var content = new StringContent($"label={order.Id}&receiver=410011494697259&quickpay-form=shop&targets=Оплата заказа {order.Id}&sum={order.Sum}&paymentType=AC&submit-button=Оплатить&successURL=https://botqually.ru/subscribe/success&quickpay-back-url=https://botqually.ru", Encoding.UTF8, "application/x-www-form-urlencoded");
                                var result = await client.PostAsync("https://money.yandex.ru/quickpay/confirm.xml", content);
                                Uri uri2 = new Uri(result.Headers.Location.ToString());
                                return Redirect(uri2.AbsoluteUri);
                            }
                        }
                    case "robo":
                        string signature = CreateMD5($"qually:{order.Sum}:0:{pass1}:shp_order={order.Id}:shp_userid={order.UserId}");
                        Uri uri1 = new Uri($"https://auth.robokassa.ru/Merchant/Index.aspx?MerchantLogin=qually&OutSum={order.Sum}&InvoiceID=0&Description={order.Description}&SignatureValue={signature}&shp_order={order.Id}&shp_userid={order.UserId}");
                        return Redirect(uri1.AbsoluteUri);
                }
            }
            return BadRequest();
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
