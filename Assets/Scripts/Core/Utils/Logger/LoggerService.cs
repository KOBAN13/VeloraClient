using System.Collections.Generic;
using Core.Utils.Logger.Data;
using R3;
using UI.Services.Data;
using UnityEngine;

namespace Core.Utils.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerParameters _parameters;
        private readonly List<ChatMessageData> _messages = new();
        private readonly Subject<ChatMessageData> _messageAdded = new();

        public Observable<ChatMessageData> MessageAdded => _messageAdded;
        public IReadOnlyCollection<ChatMessageData> Messages => _messages;

        public LoggerService(ILoggerParameters parameters)
        {
            _parameters = parameters;
        }

        public void Log(string message, string source = "System")
        {
            PushMessage(source, message, _parameters.InfoColor, ChatMessageType.Info);
        }
                    
        public void Warning(string message, string source = "System")
        {
            PushMessage(source, message, _parameters.WarningColor, ChatMessageType.Warning);
        }

        public void Error(string message, string source = "System")
        {
            PushMessage(source, message, _parameters.ErrorColor, ChatMessageType.Error);
        }

        private void PushMessage(string source, string message, Color color, ChatMessageType type)
        {
            var data = new ChatMessageData(source, message, color, type);
            _messages.Add(data);
            _messageAdded.OnNext(data);

            switch (type)
            {
                case ChatMessageType.Warning:
                    Debug.LogWarning($"[{source}] {message}");
                    break;
                case ChatMessageType.Error:
                    Debug.LogError($"[{source}] {message}");
                    break;
                default:
                    Debug.Log($"[{source}] {message}");
                    break;
            }
        }
    }
}
