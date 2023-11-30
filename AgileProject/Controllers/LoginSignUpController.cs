using AgileProject.Models;
using Microsoft.AspNetCore.Mvc;
using AgileProject.Migrations;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace AgileProject.Controllers
{
    [Route("Account")]
    public class LoginSignUpController : Controller
    {
        private readonly _dbcontext _context;
        public LoginSignUpController(_dbcontext context)
        {
            _context = context;
        }
        [Route("signup")]
        public IActionResult Signup()
        {
            if(User.Identity.IsAuthenticated)
                return Redirect("/dashboard/");
            return View("~/Views/LoginSignUpController/SignUp.cshtml");
            
        }
        [HttpPost]
        [Route("signup")]
        public IActionResult Signup(User_Signup user)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    User_ userr = new();
                    userr.Email = user.Email;
                    userr.Name = user.Name;
                    userr.PasswordHash = user.Password;
                    _context.User.Add(userr);
                    _context.SaveChanges();
                    return Redirect("/account/login");
                }
                catch (Exception e)
                {
                    ViewData["Client_reg"] = true;
                }
            }
            return View("~/Views/LoginSignUpController/SignUp.cshtml");
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/dashboard/");
            return View("~/Views/LoginSignUpController/Login.cshtml");
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(User_Credintinals user)
        {
            if (ModelState.IsValid)
            {
                User_ find = _context.User.FirstOrDefault(x => x.Email == user.Email);
                if (find != null)
                {
                    if (find.PasswordHash == user.Password)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, user.Email)
                        };
                        var claimsIdentity = new ClaimsIdentity(
                         claims,
                         CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(
                     CookieAuthenticationDefaults.AuthenticationScheme,
                         new ClaimsPrincipal(claimsIdentity));
                        return Redirect("/dashboard/");
                    }
                }
                else
                {
                    ViewData["Client_reg"] = true;
                }
            }
            return View("~/Views/LoginSignUpController/Login.cshtml");
        }
        [HttpGet]
        [Route("Logout")]
        public async Task<ActionResult> logout()
        {
            if(User.Identity.IsAuthenticated)
            await HttpContext.SignOutAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/login");
            
        }
        [Route("/facebook-sign")]
        public async Task<ActionResult> facebook()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/dashboard", }, FacebookDefaults.AuthenticationScheme);
        }
    }
}
