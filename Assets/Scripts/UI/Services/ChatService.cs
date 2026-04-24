using System.Collections.Generic;
using Core.Utils.Pool;
using UI.Services.Data;
using UI.Views;

namespace UI.Services
{
    public class ChatService : IChatService
    {
        private Queue<MessageView> _queue = new();
        
        private readonly PoolService _poolService;

        public ChatService(PoolService poolService)
        {
            _poolService = poolService;
        }

        public void AddMessage(ChatMessageData data)
        {
            var message = _poolService.Spawn<MessageView>(EObjectInPoolName.ChatMessage, true);
        }

        public void SendMessage(ChatMessageData data)
        {
            
        }
    }
}