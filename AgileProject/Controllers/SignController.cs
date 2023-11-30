using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgileProject.Controllers
{
    [AllowAnonymous]
    public class SignController : Controller
    {
        [HttpGet]
        [Route("/signin-facebook")]
        public IActionResult Facebook()
        {
            return Content("Done") ;
        }
    }
}
