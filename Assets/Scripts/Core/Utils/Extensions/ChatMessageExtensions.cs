using System;
using UI.Services.Data;

namespace Core.Utils.Extensions
{
    public static class ChatMessageExtensions
    {
        public static string FormatMessage(this ChatMessageData data)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            var type = GetTypeLabel(data.Type);

            var username = data.Username.FormatText();
            var message = data.Text.FormatText();

            return $"<size=80%>[{time}] {type}</size> <b>{username}</b>: {message}";
        }

        public static string GetTypeLabel(this ChatMessageType type)
        {
            return type switch
            {
                ChatMessageType.Warning => "[WARN]",
                ChatMessageType.Error => "[ERROR]",
                _ => "[INFO]"
            };
        }
    }
}