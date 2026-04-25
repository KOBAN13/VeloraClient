using System.Collections.Generic;
using R3;
using UI.Services.Data;

namespace UI.Services
{
    public interface IChatService
    {
        Observable<ChatMessageDataView> OnMessageAdded { get; }
        IReadOnlyCollection<ChatMessageData> Messages { get; }
        void AddMessage(ChatMessageData data);
    }
}
