using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;

namespace Qually.Api
{
    [Route("api/[controller]")]
    public class YandexController : Controller
    {
        private readonly QuallyContext db;

        public YandexController(QuallyContext context)
        {
            db = context;
        }

        [HttpPost]
        public IActionResult Post(string notification_type, string operation_id, string amount, string withdraw_amount, string currency, string datetime, string sender, bool codepro, string label, string sha1_hash, bool test_notification)
        {
            if (!test_notification)
            {
                string secretKey = "KExQB397atghem+00jCy1Mg2";
                string paramString = string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}&{7}&{8}",
                    notification_type, operation_id, amount, currency, datetime, sender, codepro.ToString().ToLower(), secretKey, label);
                string hash = GetHash(paramString);
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                if (0 == comparer.Compare(hash, sha1_hash))
                {
                    var order = db.Orders.FirstOrDefault(o => o.Id == label && !o.Paid);
                    if (order != null)
                    {
                        if (order.Sum == Convert.ToDecimal(withdraw_amount))
                        {
                            Transaction transaction = new Transaction
                            {
                                Operation_Id = operation_id,
                                Date = DateTime.Now,
                                Sum = Convert.ToDecimal(withdraw_amount),
                                Sender = sender,
                                UserId = order.UserId,
                                Type = PayType.Yandex
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
                            return Ok();
                        }
                    }
                }
            }
            else
            {
                Transaction transaction = new Transaction
                {
                    Operation_Id = operation_id,
                    Date = DateTime.Now,
                    Sum = Convert.ToDecimal(withdraw_amount),
                    Email = sender,
                    UserId = "test",
                    Type = PayType.Yandex
                };
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        private string GetHash(string val)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(val));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
