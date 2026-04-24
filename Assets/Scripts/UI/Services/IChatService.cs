using UI.Services.Data;

namespace UI.Services
{
    public interface IChatService
    {
        void AddMessage(ChatMessageData data);
    }
}