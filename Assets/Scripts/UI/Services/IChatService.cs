using System;
using System.Collections.Generic;
using UI.Services.Data;

namespace UI.Services
{
    public interface IChatService
    {
        event Action<ChatMessageData> MessageAdded;
        IReadOnlyCollection<ChatMessageData> Messages { get; }
        void AddMessage(ChatMessageData data);
    }
}
