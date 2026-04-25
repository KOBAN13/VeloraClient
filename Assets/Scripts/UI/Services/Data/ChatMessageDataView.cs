using UI.Views;

namespace UI.Services.Data
{
    public readonly struct ChatMessageDataView
    {
        public readonly ChatMessageData Data;
        public readonly MessageView InstantiateView;

        public ChatMessageDataView(MessageView instantiateView, ChatMessageData data)
        {
            InstantiateView = instantiateView;
            Data = data;
        }
    }
}