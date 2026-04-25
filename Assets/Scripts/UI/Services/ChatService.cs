using System.Collections.Generic;
using Core.Utils.Pool;
using Network;
using R3;
using UI.Services.Data;
using UI.Views;

namespace UI.Services
{
    public class ChatService : IChatService
    {
        private readonly IWebsocketConnectionService _websocketConnectionService;
        
        private readonly List<ChatMessageData> _messages = new();
        private readonly IPoolService _poolService;
        
        private readonly Subject<ChatMessageDataView> _subject = new();
        
        public Observable<ChatMessageDataView> OnMessageAdded => _subject.AsObservable();
        public IReadOnlyCollection<ChatMessageData> Messages => _messages;

        public ChatService(IPoolService poolService, IWebsocketConnectionService websocketConnectionService)
        {
            _poolService = poolService;
            _websocketConnectionService = websocketConnectionService;
        }

        public void AddMessage(ChatMessageData data)
        {
            var instantiateMessage = _poolService.Spawn<MessageView>(EObjectInPoolName.ChatMessage, false);

            var dataView = new ChatMessageDataView(instantiateMessage, data);
            
            _messages.Add(data);
            _subject.OnNext(dataView);
        }

        public void SendMessage(ChatMessageData data)
        {
            AddMessage(data);
            
            _websocketConnectionService.PrepareNewPackage(data.Text);
        }
    }
}
