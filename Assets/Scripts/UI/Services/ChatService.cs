using System.Collections.Generic;
using Core.Utils.Pool;
using UI.Services.Data;
using UI.Views;

namespace UI.Services
{
    public class ChatService : IChatService
    {
        private readonly Queue<MessageView> _queue = new();
        private readonly List<ChatMessageData> _messages = new();
        private readonly PoolService _poolService;

        public event System.Action<ChatMessageData> MessageAdded;
        public IReadOnlyCollection<ChatMessageData> Messages => _messages;

        public ChatService(PoolService poolService)
        {
            _poolService = poolService;
        }

        public void AddMessage(ChatMessageData data)
        {
            _messages.Add(data);
            MessageAdded?.Invoke(data);
        }

        public void SendMessage(ChatMessageData data)
        {
            AddMessage(data);
        }
    }
}
