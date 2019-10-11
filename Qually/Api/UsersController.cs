using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuallyLib;

namespace Qually.Api
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly QuallyContext db;

        public UsersController(QuallyContext db)
        {
            this.db = db;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddUser([FromForm]string id)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user == null && !id.Contains("ver 9.") && id.Length == 20)
            {
                user = new User
                {
                    PCID = id,
                    Lvl = SubscriptionLvl.Level_No,
                    SubscriptionDate = new DateTime(2019, 1, 1),
                    SubscriptionExpDate = new DateTime(2019, 1, 2),
                    Active = false,
                    TrialUse = false,
                    Trial = false,
                    UnlimitedSub = false
                };
                db.Users.Add(user);
                db.SaveChanges();
                return Ok("User added!");
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("trial")]
        public IActionResult UseTrial([FromForm]string id)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                if (!user.TrialUse)
                {
                    user.TrialUse = true;
                    user.Trial = true;
                    user.SubscriptionDate = DateTime.Now;
                    user.SubscriptionExpDate = DateTime.Now.AddDays(5);
                    user.Lvl = SubscriptionLvl.Level_2;
                    user.Active = true;
                    db.Users.Update(user);
                    db.SaveChanges();
                    return Ok("Trial activated!");
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult EditUser([FromForm]string oldId, [FromForm]string newId)
        {
            if (!newId.Contains("ver 9.") && newId.Length == 64)
            {
                var user = db.Users.FirstOrDefault(u => u.PCID == oldId);
                if (user != null)
                {
                    user.PCID = newId;
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PCID == id);
            if (user != null)
            {
                if (!user.UnlimitedSub)
                {
                    if (user.SubscriptionExpDate < DateTime.Now)
                    {
                        user.Active = false;
                        user.Trial = false;
                        user.Lvl = SubscriptionLvl.Level_No;
                        db.Users.Update(user);
                        db.SaveChanges();
                    }
                }
                user.Hash = MD5hash($"{user.PCID}|{user.SubscriptionExpDate.ToString("dd.MM.yyyy")}|{user.UnlimitedSub}|{user.Trial}|{(int)user.Lvl}|{user.Active}|helicopter").ToLower();
                return Json(user);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("usb")]
        public IActionResult GetUserByUSB(string id)
        {
            var user = db.Users.FirstOrDefault(u => u.USBId == id);
            if (user != null)
            {
                if (!user.UnlimitedSub)
                {
                    if (user.SubscriptionExpDate < DateTime.Now)
                    {
                        user.Active = false;
                        user.Trial = false;
                        user.Lvl = SubscriptionLvl.Level_No;
                        db.Users.Update(user);
                        db.SaveChanges();
                    }
                }
                user.Hash = MD5hash($"{user.PCID}|{user.SubscriptionExpDate.ToString("dd.MM.yyyy")}|{user.UnlimitedSub}|{user.Trial}|{(int)user.Lvl}|{user.Active}|helicopter").ToLower();
                return Json(user);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("messages")]
        public IActionResult GetMessages(string id)
        {
            User user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                var messages = db.Messages.Where(m => m.UserId == user.PCID);
                return Json(messages);
            }
            return BadRequest();
        } 

        [HttpPost]
        [Route("usbactive")]
        public IActionResult ActivateUSB([FromForm]string pcid, [FromForm]string usbId)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == pcid);
            if (user != null)
            {
                if (user.Active)
                {
                    if (!user.TrialUse)
                    {
                        user.USBId = usbId;
                        db.Users.Update(user);
                        db.SaveChanges();
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("usbdeactive")]
        public IActionResult DeactivateUSB([FromForm]string pcid)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == pcid);
            if (user != null)
            {
                if (user.Active)
                {
                    if (!user.TrialUse)
                    {
                        user.USBId = string.Empty;
                        db.Users.Update(user);
                        db.SaveChanges();
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        private string MD5hash(string input)
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
