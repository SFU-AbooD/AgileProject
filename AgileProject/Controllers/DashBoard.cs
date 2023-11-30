using AgileProject.Migrations;
using AgileProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgileProject.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("dashboard")]
    public class DashBoard : Controller
    {
        private readonly _dbcontext _context;
        public DashBoard(_dbcontext context)
        {
            _context = context;
        }

        [Route("")]
        [HttpGet]
        public  IActionResult Index()
        {
            var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
            var user = _context.User.FirstOrDefault(x=>x.Email == userEmil);
            if (user == null)
            {
                var name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                User_ ans = new User_ {
                    Email = userEmil,
                    Name = name,
                    PasswordHash = Guid.NewGuid().ToString()
                };
                _context.User.Add(ans);
                _context.SaveChanges();
                user = _context.User.FirstOrDefault(x => x.Email == userEmil);
            }

            {
                ViewData["Username"] = user.Name;
            }

            var tickets = _context.Tickets.Where(x => x.user_id == userEmil).ToList();
            ViewData["cc"] = tickets.Count;
            return View("~/Views/dashBoard/dashboard.cshtml", tickets);
        }

        [Route("")]
        [HttpPost]
        public IActionResult Index([FromForm] filter? filter)
        {
            var userEmil = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress") == true).Value;
            var user = _context.User.FirstOrDefault(x => x.Email == userEmil);
            {
                ViewData["Username"] = user.Name;
            }

            var tickets = _context.Tickets.Where(x => x.user_id == userEmil).ToList();
            if (filter != null)
            {
                if (filter.date != null && filter.name != null)
                {
                    tickets = tickets.Where(x => x.user_id == userEmil && x.Taskname.ToLower().Contains(filter.name) && filter.date == x.due).ToList();
                }
                else if (filter.date == null && filter.name != null)
                {
                    tickets = tickets.Where(x => x.user_id == userEmil && x.Taskname.ToLower().Contains(filter.name)).ToList();
                }
                else if (filter.date != null && filter.name == null)
                {
                    tickets = tickets.Where(x => x.user_id == userEmil && filter.date == x.due).ToList();
                }
            }

            return View("~/Views/dashBoard/dashboard.cshtml", tickets);
        }
    }
}
