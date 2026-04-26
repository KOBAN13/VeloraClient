using System.Collections.Generic;
using Core.Utils.Pool;
using R3;
using UI.Services.Data;
using UI.Views;

namespace UI.Services
{
    public class ChatService : IChatService
    {
        private readonly List<ChatMessageData> _messages = new();
        private readonly IPoolService _poolService;
        
        private readonly Subject<ChatMessageDataView> _subject = new();
        
        public Observable<ChatMessageDataView> OnMessageAdded => _subject.AsObservable();
        public IReadOnlyCollection<ChatMessageData> Messages => _messages;

        public ChatService(IPoolService poolService)
        {
            _poolService = poolService;
        }

        public void AddMessage(ChatMessageData data)
        {
            var instantiateMessage = _poolService.Spawn<MessageView>(EObjectInPoolName.ChatMessage, false);

            var dataView = new ChatMessageDataView(instantiateMessage, data);
            
            _messages.Add(data);
            _subject.OnNext(dataView);
        }
    }
}
