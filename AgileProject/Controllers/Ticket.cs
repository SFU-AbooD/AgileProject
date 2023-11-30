using AgileProject.Migrations;
using AgileProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace AgileProject.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class Ticket : Controller
    {
        private readonly _dbcontext _context;
        public Ticket(_dbcontext context) {
            _context = context;
        }
        [HttpPost]
        [Route("/counter")]
        public int counter([FromBody]int? count)
        {
            Console.WriteLine(count);
            var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
            if (count != null)
            {
                int v = _context.Tickets.Where(x => x.user_id == userEmil).Count();
                if (v != count)
                    return v;
            }
            return count ?? 0;
        }
        [HttpGet]
        [Route("/download/{id?}")]
        public IActionResult download(Guid? id)
        {
            if (id != null)
            {
                var ff = _context.files.Where(x => x.file_id == id).FirstOrDefault();
                if (ff != null)
                {
                    if(ff.file_extention.ToLower().Contains("png"))
                     return File(ff.file, "image/png", ff.file_name);
                    else
                        return File(ff.file, "text/plain", ff.file_name);
                }
                else
                    return NotFound();
            }
            else
                return NotFound();
        }
        [HttpGet]
        [Route("create_ticket")]
        public IActionResult create_ticket()
        {
            var users = _context.User.ToList();
            var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
            var user = _context.User.FirstOrDefault(x => x.Email == userEmil);
            {
                ViewData["Username"] = user.Name;
            }
            ViewData["users"] = users;
            return View("~/Views/Ticket_update/Ticket_create.cshtml");
        }
        [Route("/reports")]
        public IActionResult reports()
        {
            var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
            var user = _context.User.FirstOrDefault(x => x.Email == userEmil);
            {
                ViewData["Username"] = user.Name;
            }
            ViewData["todo"] = _context.Tickets.Where(x => x.Taskstatus == "todo").Count(); 
            ViewData["in-progress"] = _context.Tickets.Where(x => x.Taskstatus == "in-progress").Count(); 
            ViewData["done"] = _context.Tickets.Where(x => x.Taskstatus == "done").Count();
            var all = _context.User.ToList();
            ViewData["all"] = all;
            foreach (var i in all)
            {
                ViewData[i.Email.ToString()] = _context.Tickets.Where(x => x.user_id == i.Email && (x.Taskstatus == "todo" || x.Taskstatus == "in-progress")).ToList();
            }
            return View("~/Views/Ticket_update/reports.cshtml");
        }
        [HttpPost]
        [Route("create_ticket")]
        public IActionResult create_ticket(Tickets tick)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (tick.due < DateTime.Now)
                    {
                        TempData["wrong"] = true;
                        return RedirectToAction(nameof(create_ticket));
                    }
                    _context.Tickets.Add(tick);
                    _context.SaveChanges();
                    return Redirect("/dashboard");
                }
                catch (Exception e) {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(create_ticket));
        }
        [Route("update-status")]
        [HttpPost]
        public string update([FromBody]update_ticket ticket)
        {
           var tick =  _context.Tickets.FirstOrDefault(x => x.id == ticket.ticket_id);
            if (tick != null)
            {
                var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
                string query = $"user {userEmil} has changed task status from {tick.Taskstatus} to {ticket.status}";
                tick.Taskstatus = ticket.status;
                updates up = new()
                {
                    ticket_id = ticket.ticket_id,
                    update_id = new Guid(),
                    message = query,
                    user_id = userEmil
                };
                _context.updates.Add(up);
                _context.SaveChanges();
                return "changed";
            }
            else
                return "bad";
        }
        [Route("add/comment")]
        [HttpPost]
        public IActionResult add_comment(comments? comment)
        {
            if (comment != null && comment.data.Length > 0)
            {
                var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
                comment.user_id = userEmil;
                comment.comment_id = new Guid();
                _context.comments.Add(comment);
                _context.SaveChanges();
                return Redirect("/dashboard");
            }
            else
                return NotFound();
        }
        [Route("taskDetails/{id?}")]
        public IActionResult Details(Guid? id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return NotFound();
            var tick = _context.Tickets.FirstOrDefault(x => x.id == id);
            var users = _context.User.ToList();
            var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
            var user = _context.User.FirstOrDefault(x => x.Email == userEmil);
            ViewData["Username"] = user.Name;
            ViewData["users"] = users;
            var comments = _context.comments.Where(x => x.ticket_id == id).ToList();
            var updates = _context.updates.Where(x => x.ticket_id == id).ToList();
            var files = _context.files.Where(x => x.ticket_id == id).ToList();
            if (tick != null)
            {
                ViewData["ticket"] = tick;
                ViewData["updates"] = updates;
                ViewData["comments"] = comments;
                ViewData["files"] = files;
                return View("~/Views/Ticket_update/Ticket_Details.cshtml",tick);
            }
            else
                return NotFound();
        }
        [Route("update")]
        [HttpPost]
        public IActionResult Details(Tickets? update,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        files ff = new files()
                        {
                            file = stream.ToArray(),
                            file_name = file.FileName,
                            file_extention = System.IO.Path.GetExtension(file.FileName),
                            file_id = new Guid(),
                            ticket_id = update.id
                        };
                         _context.files.Add(ff);
                        _context.SaveChanges();
                    };
                }
                var tick = _context.Tickets.FirstOrDefault(x => x.id == update.id);
                tick.user_id = update.user_id;
                tick.due = update.due;
                tick.Taskname = update.Taskname;
                _context.SaveChanges();
                return Redirect("/dashboard");
            }
            return NotFound();
        }
    }
}
