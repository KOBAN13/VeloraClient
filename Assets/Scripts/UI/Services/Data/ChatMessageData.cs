using UnityEngine;

namespace UI.Services.Data
{
    public readonly struct ChatMessageData
    {
        public readonly string Username;
        public readonly string Text;
        public readonly Color Color;

        public ChatMessageData(string username, string text, Color color)
        {
            Username = username;
            Text = text;
            Color = color;
        }
    }
}