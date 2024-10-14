using CoffeeHouse.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class HomeController : Controller
    {
        private readonly AccountService _accountService;

        public HomeController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
           
            return View();
        }


    }
}
