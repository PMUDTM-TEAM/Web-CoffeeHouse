using CoffeeHouse.Models;
using CoffeeHouse.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.User
{
    public class ProductOrder
    {
        public int Provar_Id { get; set; }
        public List<int> Topping_Ids { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class CartController : Controller
    {
        private readonly ProductVariantService productVariantService;
        private readonly CartService cartService;
        private readonly ToppingService toppingService;
        private readonly SizeService sizeService;
        private readonly ProductService productService;
        private readonly AddressService addressService;
        private readonly OrderService orderService;

        public CartController(ProductVariantService _productVariantService, OrderService _orderService, AddressService _addressService, ProductService _productService, CartService _cartService, ToppingService _toppingService, SizeService _sizeService)
        {
            this.productVariantService = _productVariantService;
            this.cartService = _cartService;
            this.toppingService = _toppingService;
            this.sizeService = _sizeService;
            this.productService = _productService;
            this.addressService = _addressService;
            this.orderService = _orderService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.CheckLogin = 1;
            var userId = HttpContext.Session.GetString("UserId");


            // Kiểm tra nếu userId không tồn tại (người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                // Trả về thông báo yêu cầu đăng nhập
                ViewBag.CheckLogin = 0;
                return View();
            }
            int countCart = await cartService.countCartByAId(int.Parse(userId));
            ViewBag.CountCart = countCart;
            var products = await productService.GetAllAsync();
            var toppings = await toppingService.GetAllToppings();
            var productVariants = await productVariantService.GetAllProductVariantsAsync();
            var sizes = await sizeService.GetAllSizes();
            int A_Id = int.Parse(userId);
            var carts = await cartService.GetAllCartByIdAsync(A_Id);

            var cartDetails = from cart in carts
                              join productVariant in productVariants on cart.Provar_Id equals productVariant.Id
                              join size in sizes on productVariant.Size_Id equals size.Id
                              join product in products on productVariant.Pro_Id equals product.Id
                              select new
                              {
                                  Cart = cart,
                                  ProductVariant = productVariant,
                                  Size = size,
                                  Product = product,
                              };
            ViewBag.Carts = carts;
            ViewBag.productVariants = productVariants;
            ViewBag.sizes = sizes;
            ViewBag.toppings = toppings;
            ViewBag.CartDetails = cartDetails;

            return View();
           
        }

        public async Task<IActionResult> removeCart(int Cart_Id)
        {
            int checkRemoveTopping = await cartService.RemoveCartToppingByCartIdAsync(Cart_Id);
            int checkRemove = await cartService.RemoveFromCartByIdAsync(Cart_Id);

            if (checkRemove == 1)
            {
                return Json(new { success = true, message = "Xóa thành công sản phẩm khỏi giỏ hàng." });
            }
            else
            {
                return Json(new { success = false, message = "Xóa sản phẩm không thành công." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = HttpContext.Session.GetString("UserId");
            int noLoginCount = 0;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = true, noLoginCount });
            }
            int count = await cartService.countCartByAId(int.Parse(userId));

            return Json(new { success = true, count });
        }

        [HttpPost]
        public async Task<IActionResult> addItemToCart(int productId, decimal price, int sizeId, List<int> toppings, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");


            // Kiểm tra nếu userId không tồn tại (người dùng chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                // Trả về chỉ thị chuyển hướng
                return Json(new { success = false, redirect = true, redirectUrl = Url.Action("Index", "Account"), message = "Bạn cần đăng nhập để thực hiện mua hàng." });
            }


            var Provar_Id = await productVariantService.GetProvarByProIdAndSizeId(productId, sizeId);
            if (Provar_Id == -1)
            {
                return Json(new { success = false, message = "Mã sản phẩm không có, đang bị lỗi." });
            }
            var cartItem = new Cart
            {
                
                Provar_Id = Provar_Id,
                A_Id = int.Parse(userId),
                Quantity = quantity,
                TotalPrice =  price * quantity,
                
            };

            await cartService.AddToCartAsync(cartItem); // Đảm bảo rằng bạn đang gọi phương thức bất đồng bộ
            var cartId = await cartService.GetCartIdMaxByAIdAsync(int.Parse(userId));
            // Thêm từng topping vào bảng CartTopping
            foreach (var toppingId in toppings)
            {
                await cartService.addToCartTopping(cartId, toppingId);
            }

            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng." });
        }

        [HttpPost]
        public async Task<IActionResult> placeOrder(string address, string district, string ward, string phone, decimal cartPrice, List<ProductOrder> products)
        {
            var userId = HttpContext.Session.GetString("UserId");
            int A_Id = int.Parse(userId);
            //int Address_Id = await addressService.getAddressIdMax() + 1;
            var addresses = new Addresses
            { 
                Address = address,
                A_ID = int.Parse(userId),
                District = district,
                Ward = ward,
                Phone = phone
            };
            await addressService.AddToAddressAsync(addresses);
            //int Order_Id = await orderService.getMaxIdOrder() + 1;
            int Address_Id = await addressService.GetAddressIdMaxByAIdAsync(A_Id);
            var order = new Orders
            {
              
                Date = DateTime.Now,
                Status = "Pending",
                A_Id = A_Id,
                TotalPrice = cartPrice,
                Address_Id = Address_Id,
            };
            await orderService.AddToOrderAsync(order);
            int Order_Id = await orderService.GetOrderIdMaxByAIdAsync(A_Id);
            foreach (var product in products)
            {
                // Truy cập thông tin từng sản phẩm
                int provarId = product.Provar_Id;
                List<int> toppingIds = product.Topping_Ids;
                  
                int quantity = product.Quantity;
                decimal totalPrice = product.TotalPrice;

                //int orderDetail_Id = await orderService.getMaxIdOrderDetail() + 1;
                var orderDetail = new OrderDetails
                {

                    Price = totalPrice,
                    Quantity = quantity,
                    Provar_Id = provarId,
                    Order_Id = Order_Id
                };
                await orderService.addToOrderDetail(orderDetail);
                int orderDetail_Id = await orderService.GetOrderDetailIdMaxByOrderIdAsync(Order_Id);
                foreach (var toppingId in toppingIds)
                {
                    await orderService.AddToOrderToppingAsync(orderDetail_Id, toppingId);
                }
            }

            List<int> cart_Ids = await cartService.getAllCartIdByAId(A_Id);
            foreach(var cartId in cart_Ids)
            {
                int checkRemove=await cartService.RemoveCartToppingByCartIdAsync(cartId);
            }
            await cartService.DeleteCartsByAIdAsync(A_Id);
            return Json(new { success = true, message = "Đặt đơn hàng thành công" });

        }
    }
}
