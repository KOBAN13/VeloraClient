using Core.Utils.Logger.Data;
using UI.Services;
using UI.Services.Data;
using UnityEngine;

namespace Core.Utils.Logger
{
    public class LoggerService : ILoggerService
    {
        private ILoggerParameters _parameters;

        private readonly IChatService _chatService;

        public LoggerService(IChatService chatService, ILoggerParameters parameters)
        {
            _chatService = chatService;
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
            _chatService.AddMessage(new ChatMessageData(source, message, color, type));
        }
    }
}
