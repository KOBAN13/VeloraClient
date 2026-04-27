using System.Collections.Generic;
using R3;
using UI.Services.Data;

namespace UI.Services
{
    public class ChatService : IChatService
    {
        private readonly List<ChatMessageData> _messages = new();
        
        private readonly Subject<ChatMessageData> _subject = new();
        
        public Observable<ChatMessageData> OnMessageAdded => _subject.AsObservable();
        public IReadOnlyCollection<ChatMessageData> Messages => _messages;

        public void AddMessage(ChatMessageData data)
        {
            _messages.Add(data);
            _subject.OnNext(data);
        }
    }
}
