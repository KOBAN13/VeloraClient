using System.Collections.Generic;
using R3;

namespace UI.Services.Data
{
    public class ChatMessageFeed
    {
        public readonly IReadOnlyCollection<ChatMessageData> messages;
        public readonly Observable<ChatMessageData> onMessageAdded;
        
        public ChatMessageFeed(IReadOnlyCollection<ChatMessageData> messages, Observable<ChatMessageData> onMessageAdded)
        {
            this.messages = messages;
            this.onMessageAdded = onMessageAdded;
        }
    }
}
