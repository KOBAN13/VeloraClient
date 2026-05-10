using System.Collections.Generic;
using R3;
using UI.Services.Data;

namespace Core.Utils.Logger
{
    public interface ILoggerService
    {
        Observable<ChatMessageData> MessageAdded { get; }
        IReadOnlyCollection<ChatMessageData> Messages { get; }

        void Log(string message, string source = "System");
        void Warning(string message, string source = "System");
        void Error(string message, string source = "System");
    }
}
