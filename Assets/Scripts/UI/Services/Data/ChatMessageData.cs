using UnityEngine;

namespace UI.Services.Data
{
    public readonly struct ChatMessageData
    {
        public readonly string Username;
        public readonly string Text;
        public readonly Color Color;
        public readonly ChatMessageType Type;

        public ChatMessageData(string username, string text, Color color, ChatMessageType type = ChatMessageType.Info)
        {
            Username = username;
            Text = text;
            Color = color;
            Type = type;
        }
    }
}
