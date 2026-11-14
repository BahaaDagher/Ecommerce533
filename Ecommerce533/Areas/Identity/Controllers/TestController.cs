using Microsoft.AspNetCore.Mvc;

namespace Ecommerce533.Areas.Identity.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
