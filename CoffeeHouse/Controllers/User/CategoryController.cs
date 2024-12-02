using CoffeeHouse.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class CategoryController : Controller
    {
        private readonly ProductService proService;
        private readonly ProductVariantService provarService;
        private readonly SizeService sizeService;
        private readonly CategoryService cateService;
        private readonly ToppingService toppingService;
        public CategoryController(ProductService _proService, ProductVariantService _provarService, SizeService _sizeService, ToppingService _toppingService, CategoryService _cateService) 
        {
            proService = _proService;
            provarService = _provarService;
            sizeService = _sizeService;
            cateService = _cateService;
            toppingService = _toppingService;
            
        }


        public async Task<IActionResult> Index(string type)
        {
            var allTopping = await toppingService.GetAllToppings();
            var allSize = await sizeService.GetAllSizes();
            var cateCount = await cateService.CountAllCategories();
            var productVariants = await provarService.GetAllProductVariantsAsync();
            int CateId = await cateService.GetIdBySlugAsync(type);
            Console.WriteLine(CateId);

            var products = await proService.GetProductByCateIdAsync(CateId);
            var provarSize = from pv in productVariants
                             join s in allSize on pv.Size_Id equals s.Id
                             select new
                             {
                                 ProductVariant = pv,
                                 Size = s
                             };
            var combined = from pv in productVariants
                           join p in products on pv.Pro_Id equals p.Id
                           where pv.Size_Id == 1 // Lọc các ProductVariant có Size_Id = 1
                           select new { Product = p, ProductVariant = pv };

            ViewBag.Toppings = allTopping;
            ViewBag.ProSize = provarSize;
            ViewBag.ProductVariant = productVariants;
            ViewBag.CateCount = cateCount;
            ViewBag.ProductVariantsWithProducts = combined.ToList();
            return View();
        }
    }
}
