using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;

namespace Qually.Api
{
    [Route("api/[controller]")]
    public class RoboController : Controller
    {
        private readonly QuallyContext db;

        public RoboController(QuallyContext db)
        {
            this.db = db;
        }

        private readonly string testPass2 = "V5UUtp9FlvUDe2H38phc";
        private readonly string pass2 = "itT13gC2VKDwgrpV89dQ";
        [HttpGet]
        public IActionResult Get(string OutSum, string InvId, string Email, string Fee, string SignatureValue, string shp_order, string shp_userid)
        {
            if (SignatureValue == CreateMD5($"{OutSum}:{InvId}:{pass2}:shp_order={shp_order}:shp_userid={shp_userid}"))
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == shp_order && !o.Paid);
                if (order != null)
                {
                    if (order.Sum == Convert.ToDecimal(OutSum))
                    {
                        Transaction transaction = new Transaction
                        {
                            Operation_Id = InvId,
                            Date = DateTime.Now,
                            Sum = Convert.ToDecimal(OutSum),
                            UserId = shp_userid,
                            Email = Email,
                            Type = PayType.Robokassa
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
                        return Content("OK");
                    }
                }
            }
            return BadRequest();
        }

        private string CreateMD5(string input)
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
