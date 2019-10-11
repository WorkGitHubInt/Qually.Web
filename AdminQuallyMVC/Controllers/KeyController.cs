using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdminQuallyMVC.Controllers
{
    [Authorize]
    public class KeyController : Controller
    {
        private readonly QuallyContext db;

        public KeyController(QuallyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View(db.Keys.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Generate(string userId, SubscriptionLvl lvl, int duration)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == userId);
            if (user != null)
            {
                Key key = new Key
                {
                    Code = CreateMD5($"{userId}{lvl}{duration}{RandomString(4)}"),
                    UserId = userId,
                    Activated = false,
                    Lvl = lvl,
                    Duration = duration,
                };
                db.Keys.Add(key);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти пользователя {userId}!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Key key = await db.Keys.FindAsync(id);
                if (key != null)
                {
                    db.Keys.Remove(key);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Error", new { message = $"Не удалось найти ключ {id}!" });
        }

        private string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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
