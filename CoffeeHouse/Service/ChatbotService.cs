using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace OrderHighLand.Service
{
    public class ChatbotService : ActivityHandler
    {
        private readonly ILogger<ChatbotService> _logger;

        public ChatbotService(ILogger<ChatbotService> logger)
        {
            _logger = logger;
        }

        // Hàm xử lý tin nhắn từ người dùng và phản hồi
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy nội dung tin nhắn từ người dùng và chuyển sang chữ thường để tiện xử lý
                string userMessage = turnContext.Activity.Text?.ToLower().Trim();

                // Nếu tin nhắn trống hoặc null, bot sẽ phản hồi một thông báo mặc định
                if (string.IsNullOrEmpty(userMessage))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Xin vui lòng nhập tin nhắn."), cancellationToken);
                    return;
                }

                // Tạo phản hồi cho người dùng
                var replyText = $"Bạn vừa nói: {turnContext.Activity.Text}";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText), cancellationToken);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để theo dõi
                _logger.LogError(ex, "Error in OnMessageActivityAsync");

                // Phản hồi lỗi cho người dùng
                await turnContext.SendActivityAsync(
                    MessageFactory.Text("Đã xảy ra lỗi trong quá trình xử lý yêu cầu của bạn. Xin vui lòng thử lại sau."),
                    cancellationToken
                );
            }
        }

        // Hàm chào mừng người dùng mới tham gia cuộc trò chuyện
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Xin chào! Tôi là bot của bạn!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }

}
