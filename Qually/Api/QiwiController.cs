using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Utils;
using QuallyLib;

namespace Qually.Api
{
    [Route("api/[controller]")]
    public class QiwiController : Controller
    {
        private readonly QuallyContext db;

        public QiwiController(QuallyContext context)
        {
            db = context;
        }

        [HttpPost]
        public IActionResult Post([FromBody]dynamic notify)
        {
            string hash = Request.Headers["X-Api-Signature-SHA256"];
            string merchantSecret = "eyJ2ZXJzaW9uIjoiUDJQIiwiZGF0YSI6eyJwYXlpbl9tZXJjaGFudF9zaXRlX3VpZCI6ImRiMzQ2OTIzLTgxNjQtNDA0NC1hNmE2LWMzMDhiODU0NjE0YyIsInVzZXJfaWQiOiI3OTAwNTY4ODQxNyIsInNlY3JldCI6IjdkNWY4NTQ4OGY2ZmE4MzhhMTQ3MjMxZDI5MTRlODg1ZTQ5MTZhMWIzZDFmMzM0MWJlNWI5NTExYzZmNzU5M2IifX0=";
            var notification = new Notification
            {
                Bill = new Bill
                {
                    SiteId = notify.bill.siteId,
                    BillId = notify.bill.billId,
                    Amount = new MoneyAmount
                    {
                        ValueDecimal = Convert.ToDecimal(notify.bill.amount.value),
                        CurrencyEnum = CurrencyEnum.Rub
                    },
                    Status = new BillStatus
                    {
                        ValueEnum = BillStatusEnum.Paid
                    }
                },
                Version = "1"
            };
            if (BillPaymentsUtils.CheckNotificationSignature(hash, notification, merchantSecret))
            {
                string id = notify.bill.billId;
                var order = db.Orders.FirstOrDefault(o => o.Id == id && !o.Paid);
                if (order != null)
                {
                    if (order.Sum == notification.Bill.Amount.ValueDecimal)
                    {
                        var transaction = new Transaction
                        {
                            Operation_Id = order.Id,
                            Date = DateTime.Now,
                            Sum = Convert.ToDecimal(notification.Bill.Amount.ValueDecimal),
                            UserId = order.UserId,
                            Type = PayType.Qiwi
                        };
                        db.Transactions.Add(transaction);
                        var user = db.Users.FirstOrDefault(u => u.PCID == order.UserId);
                        user.Lvl = order.Lvl;
                        if (order.Type == OrderType.New)
                        {
                            user.SubscriptionDate = DateTime.Now;
                            if (user.SubscriptionExpDate > DateTime.Now && user.Active)
                            {
                                user.SubscriptionExpDate = user.SubscriptionExpDate.AddMonths(order.Duration);
                            }
                            else
                            {
                                user.SubscriptionExpDate = DateTime.Now.AddMonths(order.Duration);
                            }
                        }
                        user.Active = true;
                        user.Trial = false;
                        order.Paid = true;
                        db.Users.Update(user);
                        db.Orders.Update(order);
                        db.SaveChanges();
                        return Json(new { error = "0" });
                    }
                }
            }
            return Json(new { error = "1" });
        }

        private string GetHash256(string input, string key)
        {
            byte[] bkey = Encoding.UTF8.GetBytes(key);
            using (var hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.Default.GetBytes(input);
                return Convert.ToBase64String(hmac.ComputeHash(bstr));
            }
        }
    }
}
