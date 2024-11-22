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

        public DialogflowController(UserService userService)
        {
            _userService = userService;
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

            // Xử lý các intent khác nhau
            switch (intentName)
            {
                case "xinchao":
                    fulfillmentText = "Xin chào! Tôi có thể giúp gì cho bạn?";
                    break;

                case "GetUserInfoByEmail":
                    // Kiểm tra và lấy email từ `parameters` nếu có
                    string email = string.Empty;
                    if (queryResult.TryGetProperty("parameters", out JsonElement parameters) &&
                        parameters.TryGetProperty("email", out JsonElement emailElement) &&
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
                            email = match.Value; // Lấy email từ queryText
                            Console.WriteLine("Email found manually: " + email);
                        }
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        // Gọi UserService để lấy thông tin tài khoản theo email
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


                default:
                    fulfillmentText = "Xin lỗi, tôi chưa hiểu yêu cầu của bạn.";
                    break;
            }

            var response = new { fulfillmentText = fulfillmentText };
            return Ok(response);
        }
    }
}
