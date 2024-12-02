using CoffeeHouse.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly SizeService sizeService;
        private readonly ProductService proService;
        private readonly CategoryService categoryService;
        private readonly ProductVariantService productVariantService;
        private readonly ToppingService toppingService;

        public ProductController(ProductService productService, SizeService _sizeService, ToppingService _topping, ProductService _proService, CategoryService _categoryService, ProductVariantService _productVariantService)
        {
            _productService = productService;
            sizeService = _sizeService;
            proService = _proService;
            categoryService = _categoryService;
            productVariantService = _productVariantService;
            toppingService = _topping;
        }

        [Route("Product/Detail/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {

            var product = await _productService.GetProductBySlugAsync(slug);
            var allTopping = await toppingService.GetAllToppings();
            var allSize = await sizeService.GetAllSizes();
            var cateCount = await categoryService.CountAllCategories();
            var productVariants = await productVariantService.GetAllProductVariantsAsync();



            var provarSize = from pv in productVariants
                             join s in allSize on pv.Size_Id equals s.Id
                             select new
                             {
                                 ProductVariant = pv,
                                 Size = s
                             };
            var combined = from pv in productVariants
                           join p in product on pv.Pro_Id equals p.Id
                           where pv.Size_Id == 1 // Lọc các ProductVariant có Size_Id = 1
                           select new { Product = p, ProductVariant = pv };
            ViewBag.Toppings = allTopping;
            ViewBag.ProSize = provarSize;
            ViewBag.ProductVariant = productVariants;
            ViewBag.CateCount = cateCount;
            ViewBag.ProductVariantsWithProducts = combined.ToList();

            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
