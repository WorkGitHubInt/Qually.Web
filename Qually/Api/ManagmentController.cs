using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuallyLib;

namespace Qually.Api
{
    [Route("api/[controller]")]
    public class ManagmentController : Controller
    {
        private readonly QuallyContext db;
        private IHttpContextAccessor _accessor;

        public ManagmentController(QuallyContext context, IHttpContextAccessor accessor)
        {
            db = context;
            _accessor = accessor;
        }

        [HttpPost]
        [Route("scripts")]
        public IActionResult GetScripts([FromForm]string id)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                if (user.Active)
                {
                    return Json(db.Scripts.ToList());
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("programs")]
        public IActionResult GetPrograms()
        {
            return Json(db.Programs.ToList());
        }

        [HttpGet]
        [Route("articles")]
        public IActionResult GetArticles()
        {
            var list = db.Articles.ToList();
            list.Reverse();
            return Json(list);
        }

        [HttpPost]
        [Route("key")]
        public IActionResult PostKey([FromForm]string id, [FromForm]string code)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                var key = db.Keys.FirstOrDefault(k => k.Code == code);
                if (key != null)
                {
                    if (key.UserId == id && !key.Activated)
                    {
                        user.Lvl = key.Lvl;
                        user.SubscriptionDate = DateTime.Now;
                        user.SubscriptionExpDate = DateTime.Now.AddMonths(key.Duration);
                        user.Active = true;
                        key.Activated = true;
                        db.Users.Update(user);
                        db.Keys.Update(key);
                        db.SaveChanges();
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("versions")]
        public IActionResult GetVersions(string id)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                if (user.Active)
                {
                    return Json(db.Updates.Select(u => new {u.ProgramId, u.Version}).ToList());
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("update")]
        public IActionResult GetUpdate(string id, int progId, string version)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                if (user.Active)
                {
                    var update = db.Updates.FirstOrDefault(u => u.ProgramId == progId && u.Version == version);
                    if (update != null)
                    {
                        return Content(Convert.ToBase64String(update.Zip));
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("program")]
        public IActionResult GetProgram(int progId)
        {
            var program = db.Programs.Find(progId);
            if (program != null)
            {
                return Json(program);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("message")]
        public IActionResult Post([FromForm]string id, [FromForm]int programId, [FromForm]string type, [FromForm]string body, [FromForm]string version, [FromForm]string contacts, [FromForm]string logacc, [FromForm]string logmain, [FromForm]string logerror, [FromForm]string log, [FromForm]string status)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == id);
            if (user != null)
            {
                var program = db.Programs.Find(programId);
                if (program != null)
                {
                    var message = new Message()
                    {
                        ProgramId = programId,
                        UserId = id,
                        Type = type,
                        Body = body,
                        Version = version,
                        Contacts = contacts,
                        LogAcc = logacc,
                        LogMain = logmain,
                        LogError = logerror,
                        Log = log,
                        Status = status,
                    };
                    db.Messages.Add(message);
                    db.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("count")]
        public IActionResult Post([FromForm]string userId, [FromForm]int programId)
        {
            var user = db.Users.FirstOrDefault(u => u.PCID == userId);
            if (user != null)
            {
                var program = db.Programs.Find(programId);
                if (program != null)
                {
                    var counter = new Counter()
                    {
                        UserId = userId,
                        ProgramId = programId,
                        Date = DateTime.Now,
                        IP = _accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                    };
                    db.Counters.Add(counter);
                    db.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
