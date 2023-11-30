using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgileProject.Controllers
{
    [Route("")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RedirectController : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine(HttpContext.User);
            return Redirect("/dashboard");
        }
    }
}
