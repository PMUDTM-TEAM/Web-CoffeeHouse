using Azure;
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
        private readonly OrderService _orderService;


        [HttpGet("dynamic-entities")]
        public async Task<IActionResult> GetDynamicEntities()
        {
            try
            {
                var products = await _productService.GetAllProductNamesAsync();
                var sizes = await _productService.GetAllSizeNamesAsync();

                var dynamicEntities = new
                {
                    name = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/Dialogflow/dynamic-entities",
                    lifespanCount = 50,
                    parameters = new
                    {
                        entities = new[]
                        {
                    new
                    {
                        name = "ProductName",
                        entries = products.Select(p => new
                        {
                            value = p,
                            synonyms = new[] { p }
                        }).ToArray()
                    },
                    new
                    {
                        name = "Size",
                        entries = sizes.Select(s => new
                        {
                            value = s,
                            synonyms = new[] { s }
                        }).ToArray()
                    }
                }
                    }
                };

                return Ok(new
                {
                    fulfillmentText = "Dữ liệu dynamic entities được cập nhật thành công.",
                    outputContexts = new[] { dynamicEntities }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật dynamic entities: {ex.Message}");
            }
        }
        public DialogflowController(UserService userService, ProductService productService, ProductVariantService productVariantService, OrderService orderService)
        {
            _userService = userService;
            _productService = productService;
            _productVariantService = productVariantService;
            _orderService = orderService;
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
                case "Default Welcome Intent":
                    try
                    {
                        var products = await _productService.GetAllProductNamesAsync();
                        var sizes = await _productService.GetAllSizeNamesAsync();

                        // Tạo Dynamic Entities Context
                        var dynamicEntities = new
                        {
                            name = $"{req.GetProperty("session").GetString()}/contexts/_dialogflow_dynamic_entities_",
                        lifespanCount = 50, // thời gian tồn tại trong phiên
                            parameters = new
                            {
                                entities = new[]
                                {
                            new
                            {
                                name = "ProductName",
                                entries = products.Select(p => new
                                {
                                    value = p,
                                    synonyms = new[] { p }
                                }).ToArray()
                            },
                            new
                            {
                                name = "Size",
                                entries = sizes.Select(s => new
                                {
                                    value = s,
                                    synonyms = new[] { s }
                                }).ToArray()
                            }
                        }
                            }

                        };
                        Console.WriteLine("Response to Dialogflow:");
                        Console.WriteLine(JsonSerializer.Serialize(new
                        {
                            fulfillmentText = "Chào mừng bạn đến với CoffeeHouse! Tôi có thể giúp gì cho bạn?",
                            outputContexts = new[] { dynamicEntities }
                        }, new JsonSerializerOptions { WriteIndented = true }));

                        return Ok(new
                        {
                            fulfillmentText = "Chào mừng bạn đến với CoffeeHouse! Tôi có thể giúp gì cho bạn?",
                            outputContexts = new[] { dynamicEntities }
                        });

                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Lỗi khi tải dữ liệu dynamic entities: {ex.Message}");
                    }

                case "xinchao":
                    fulfillmentText = "Xin chào! Tôi có thể giúp gì cho bạn?";
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

                        if (parameters.TryGetProperty("productName", out JsonElement productNameElement))
                        {
                            if (productNameElement.ValueKind == JsonValueKind.Array)
                            {
                                productName = string.Join(" ", productNameElement.EnumerateArray().Select(x => x.GetString()));
                            }
                            else
                            {
                                productName = productNameElement.GetString();
                            }
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

                case "ProductType":
                    string productType = string.Empty;

                    if (queryResult.TryGetProperty("parameters", out parameters))
                    {
                        if (parameters.TryGetProperty("productType", out JsonElement productTypeElement) &&
                            productTypeElement.ValueKind == JsonValueKind.String)
                        {
                            productType = productTypeElement.GetString();
                        }
                    }

                    if (string.IsNullOrEmpty(productType))
                    {
                        fulfillmentText = "Vui lòng cung cấp loại sản phẩm (Hot, New, Best Seller) để tôi có thể giúp bạn.";
                    }
                    else
                    {
                        try
                        {

                            var products = new List<Products>();

                            if (productType.Equals("Hot", StringComparison.OrdinalIgnoreCase))
                            {
                                products = await _productService.GetFourHotProduct();
                            }
                            else if (productType.Equals("New", StringComparison.OrdinalIgnoreCase))
                            {
                                products = await _productService.GetTwoNewProduct();
                            }
                            else
                            {
                                fulfillmentText = $"Hiện tại chúng tôi chỉ hỗ trợ các loại sản phẩm: Hot, New.";
                            }

                            if (products.Any())
                            {
                                var productNames = string.Join(", ", products.Select(p => p.Name));
                                fulfillmentText = $"Các sản phẩm {productType} của chúng tôi là: {productNames}.";
                            }
                            else
                            {
                                fulfillmentText = $"Không tìm thấy sản phẩm nào thuộc loại {productType}.";
                            }
                        }
                        catch (Exception ex)
                        {
                            fulfillmentText = $"Có lỗi xảy ra khi lấy dữ liệu sản phẩm: {ex.Message}";
                        }
                    }
                    break;
                case "OrderStatus-collectEmail":
                    string email = string.Empty;

                    if (queryResult.TryGetProperty("parameters", out parameters))
                    {
                        if (parameters.TryGetProperty("email", out JsonElement emailElement) &&
                            emailElement.ValueKind == JsonValueKind.String)
                        {
                            email = emailElement.GetString();
                        }
                    }

                    if (string.IsNullOrEmpty(email))
                    {
                        fulfillmentText = "Vui lòng cung cấp email hợp lệ để kiểm tra đơn hàng.";
                    }
                    else
                    {
                        try
                        {
                            var orders = await _orderService.GetOrdersByEmailAsync(email);
                            if (orders.Any())
                            {
                                var orderDetails = string.Join("\n\n", orders.Select(o =>
        $"🔹 **Đơn hàng #{o.Id}:**\n" +
        $"- **Trạng thái:** {o.Status}\n" +
        $"- **Tổng tiền:** {o.TotalPrice:N0} VND\n" +
        $"- **Ngày đặt:** {o.CreatedAt:dd/MM/yyyy}"
    ));
                                fulfillmentText = $"Danh sách đơn hàng của bạn:\n{orderDetails}";
                            }
                            else
                            {
                                fulfillmentText = $"Không tìm thấy đơn hàng nào liên kết với email {email}.";
                            }
                        }
                        catch (Exception ex)
                        {
                            fulfillmentText = $"Có lỗi xảy ra khi kiểm tra đơn hàng: {ex.Message}";
                        }
                    }
                    break;


                default:
                    fulfillmentText = "Xin lỗi, tôi chưa hiểu yêu cầu của bạn.";
                    break;
            }

            var Response = new { fulfillmentText = fulfillmentText };
            return Ok(Response);
        }

    }
}
