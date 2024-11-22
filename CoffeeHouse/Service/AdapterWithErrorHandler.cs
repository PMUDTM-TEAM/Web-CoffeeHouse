using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace CoffeeHouse.Service
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
             : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log error chi tiết
                logger.LogError(exception, "Exception caught in AdapterWithErrorHandler");

                // Gửi thông báo lỗi cho người dùng
                await turnContext.SendActivityAsync("Xin lỗi, đã xảy ra lỗi. Vui lòng thử lại.");
            };
        }

    }

}
