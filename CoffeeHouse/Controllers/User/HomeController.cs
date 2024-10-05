using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
