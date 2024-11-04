using CoffeeHouse.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class HomeController : Controller
    {

        private readonly ProductService proService;
        private readonly ProductVariantService provarService;

        // Khởi tạo controller với ToppingService
        public HomeController(ProductService _proService, ProductVariantService _provarService)
        {
            proService = _proService;
            provarService = _provarService;
        }

        public async Task<IActionResult> Index()
        {
            var hotProducts = await proService.GetFourHotProduct();
            var newProducts = await proService.GetTwoNewProduct();
            var productVariantSizeS = await provarService.GetProductVariantsBySizeId();
            var hotProductVariants = from pv in productVariantSizeS
                                     join hp in hotProducts on pv.Pro_Id equals hp.Id
                                     select new
                                     {
                                         Variant = pv,
                                         Product = hp
                                     };

            // Kết hợp sản phẩm mới với ProductVariant
            var newProductVariants = from pv in productVariantSizeS
                                     join np in newProducts on pv.Pro_Id equals np.Id
                                     select new
                                     {
                                         Variant = pv,
                                         Product = np
                                     };


            ViewBag.NewProducts = newProductVariants.ToList();
            ViewBag.HotProducts = hotProductVariants.ToList();
            return View();
        }
    }
}
