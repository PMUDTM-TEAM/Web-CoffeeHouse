using Microsoft.AspNetCore.Mvc;
using CoffeeHouse.Controllers.User;
using CoffeeHouse.Models;
using CoffeeHouse.Models.VM;
using CoffeeHouse.Service;


namespace CoffeeHouse.Controllers.Admin
{
	public class AdminDashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly AccountService _accountService;
		public AdminDashboardController(DashboardService dashboardService, AccountService accountService)
        {
            _dashboardService = dashboardService;
			_accountService = accountService;
		}

        public async Task<IActionResult> Index()
		{
			//var totalUser = await _dashboardService.GetToTalUser();
   //         var totalOrder = await _dashboardService.getTotalOrder();
   //         var totalOrderToday = await _dashboardService.getTotalAmountToday();
   //         var getOrderToday = await _dashboardService.getOrderToday();
   //         var getTotalOrderMonth = await _dashboardService.getTotalOrderMonth();
   //         var getTotalOrderYear = await _dashboardService.getTotalOrderYear();
   //         var totalCate = await _dashboardService.getTotalCategory();
   //         var totalProduct = await _dashboardService.getTotalProduct();
   //         var getOrderPending = await _dashboardService.getOrderPending();
   //         var getOrderCancel = await _dashboardService.getOrderCancel();
   //         ViewBag.TotalUser = totalUser;
   //         ViewBag.TotalOrder = totalOrder;
   //         ViewBag.TotalOrderToday = totalOrderToday;
   //         ViewBag.GetOrderToday = getOrderToday;
   //         ViewBag.GetTotalOrderMonth = getTotalOrderMonth;
   //         ViewBag.GetTotalOrderYear = getTotalOrderYear;
   //         ViewBag.GetTotalCate = totalCate;
   //         ViewBag.GetTotalProduct = totalProduct;
   //         ViewBag.GetOrderPending = getOrderPending;
   //         ViewBag.GetOrderCancel = getOrderCancel;
            return View();
		}

  //      public async Task<IActionResult> GetAllOrder()
		//{
  //          var order =  await _dashboardService.GetOrders();
		//	return View(order);
		//}
		//public async Task<IActionResult> GetAllOrderPending()
		//{
		//	var orderPending = await _dashboardService.GetAllOrderPending();
		//	return View(orderPending);
		//}
		//[HttpPost]
		//public async Task<IActionResult> UpdateOrderStatus(int id, string status)
		//{
		//	var order = await _dashboardService.ChangeOrderStatus(id, status);

		//	if (order != null)
		//	{
		//		return Json(new { success = true, message = "Order status updated successfully." });
		//	}
		//	else
		//	{
		//		return Json(new { success = false, message = "Failed to update order status." });
		//	}
		//}


	}
}
