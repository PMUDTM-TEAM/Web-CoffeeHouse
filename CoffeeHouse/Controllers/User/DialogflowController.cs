using CoffeeHouse.Models;
using CoffeeHouse.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CoffeeHouse.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class DialogflowController : Controller
    {
        private readonly UserService _userService;
        private readonly ProductVariantService _productVariantService;
        private readonly ProductService _productService;

        public DialogflowController(UserService userService, ProductService productService, ProductVariantService productVariantService)
        {
            _userService = userService;
            _productService = productService;
            _productVariantService = productVariantService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement req)
        {
            // Log toàn bộ nội dung yêu cầu để kiểm tra
            Console.WriteLine("Request from Dialogflow: " + req.ToString());

            if (!req.TryGetProperty("queryResult", out JsonElement queryResult) || queryResult.ValueKind == JsonValueKind.Null)
            {
                return BadRequest("Invalid request from Dialogflow");
            }

            if (!queryResult.TryGetProperty("intent", out JsonElement intent) || intent.ValueKind == JsonValueKind.Null)
            {
                return BadRequest("Invalid request from Dialogflow");
            }

            string intentName = intent.GetProperty("displayName").GetString();
            string fulfillmentText;
            JsonElement parameters;

            // Xử lý các intent khác nhau
            switch (intentName)
            {
                case "xinchao":
                    fulfillmentText = "Xin chào! Tôi có thể giúp gì cho bạn?";
                    break;

                case "GetUserInfoByEmail":
                    string email = string.Empty;

                    // Lấy `parameters` một lần duy nhất
                    if (queryResult.TryGetProperty("parameters", out parameters))
                    {
                        // Kiểm tra email trong parameters
                        if (parameters.TryGetProperty("email", out JsonElement emailElement) &&
                            emailElement.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(emailElement.GetString()))
                        {
                            email = emailElement.GetString();
                        }
                        else
                        {
                            // Nếu không có email trong parameters, kiểm tra thủ công trong `queryText` bằng Regex
                            string queryText = queryResult.GetProperty("queryText").GetString();
                            var match = Regex.Match(queryText, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");

                            if (match.Success)
                            {
                                email = match.Value;
                                Console.WriteLine("Email found manually: " + email);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        var account = await _userService.GetUserByEmailAsync(email);
                        if (account != null)
                        {
                            fulfillmentText = $"Thông tin tài khoản: Tên - {account.A_NAME}, Email - {account.A_EMAIL}, Role ID - {account.ROLE_ID}.";
                        }
                        else
                        {
                            fulfillmentText = "Không tìm thấy tài khoản với email này.";
                        }
                    }
                    else
                    {
                        fulfillmentText = "Không nhận diện được email. Vui lòng thử lại.";
                    }
                    break;

                case "information":
                    try
                    {
                        var products = await _productService.GetAllAsync();
                        if (products.Count > 0)
                        {
                            var productNames = string.Join(", ", products.Select(p => p.Name));
                            fulfillmentText = $"Chúng tôi có các loại nước sau: {productNames}.";
                        }
                        else
                        {
                            fulfillmentText = "Hiện tại không có sản phẩm nào trong kho.";
                        }
                    }
                    catch (Exception ex)
                    {
                        fulfillmentText = $"Có lỗi khi lấy dữ liệu sản phẩm: {ex.Message}";
                    }
                    break;

                case "ProductPrice":
                    string productName = string.Empty;
                    string size = string.Empty;

      
                    if (queryResult.TryGetProperty("parameters", out parameters))
                    {
           
                        if (parameters.TryGetProperty("productName", out JsonElement productNameElement) &&
                            productNameElement.ValueKind == JsonValueKind.String)
                        {
                            productName = productNameElement.GetString();
                        }

                        if (parameters.TryGetProperty("size", out JsonElement sizeElement) &&
                            sizeElement.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(sizeElement.GetString()))
                        {
                            size = sizeElement.GetString();  
                        }
                    }

                    if (string.IsNullOrEmpty(productName))
                    {
                        fulfillmentText = "Vui lòng cung cấp tên sản phẩm để tôi có thể tìm giá cho bạn.";
                    }
                    else if (string.IsNullOrEmpty(size))
                    {
  
                        var sizes = await _productVariantService.GetSizesByProductNameAsync(productName);

                        if (sizes.Any())
                        {
                            var sizeList = string.Join(", ", sizes);
                            fulfillmentText = $"Sản phẩm {productName} có các kích thước sau: {sizeList}. Bạn muốn xem giá của kích thước nào?";
                        }
                        else
                        {
                            fulfillmentText = $"Không tìm thấy kích thước nào cho sản phẩm {productName}.";
                        }
                    }
                    else
                    {
                        var productPrice = await _productService.GetProductPriceByNameAndSizeAsync(productName, size);
                        if (productPrice.HasValue)
                        {
                            fulfillmentText = $"Giá của sản phẩm {productName} kích cỡ {size} là {productPrice.Value} VND.";
                        }
                        else
                        {
                            fulfillmentText = $"Không tìm thấy giá của sản phẩm {productName} kích cỡ {size}.";
                        }
                    }
                    break;

                case "SizeProduct": 
                    string productNameSize = string.Empty;  

                    if (queryResult.TryGetProperty("parameters", out parameters))
                    {
                        if (parameters.TryGetProperty("productNameSize", out JsonElement productNameElement) &&
                            productNameElement.ValueKind == JsonValueKind.String)
                        {
                            productNameSize = productNameElement.GetString(); 
                        }
                    }

                    if (!string.IsNullOrEmpty(productNameSize))
                    {
                        var sizes = await _productVariantService.GetSizesByProductNameAsync(productNameSize);

                        if (sizes.Any())
                        {
                            var sizeList = string.Join(", ", sizes);
                            fulfillmentText = $"Các kích cỡ của sản phẩm {productNameSize} là: {sizeList}.";
                        }
                        else
                        {
                            fulfillmentText = $"Không tìm thấy kích cỡ nào cho sản phẩm {productNameSize}.";
                        }
                    }
                    else
                    {
                        fulfillmentText = "Vui lòng cung cấp tên sản phẩm để tôi có thể tìm kích cỡ cho bạn.";
                    }
                    break;




                default:
                    fulfillmentText = "Xin lỗi, tôi chưa hiểu yêu cầu của bạn.";
                    break;
            }

            var response = new { fulfillmentText = fulfillmentText };
            return Ok(response);
        }
    }
}
