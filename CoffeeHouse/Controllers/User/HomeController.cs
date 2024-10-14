using CoffeeHouse.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class HomeController : Controller
    {

        private readonly ToppingService _toppingService;

        // Khởi tạo controller với ToppingService
        public HomeController()
        {
            _toppingService = new ToppingService();
        }

        public async Task<IActionResult> Index()
        {
            // Gọi hàm GetAllToppings để lấy danh sách topping
            var toppings = await _toppingService.GetAllToppings();
            ViewBag.Toppings = toppings;
            
            return View();
        }
    }
}
