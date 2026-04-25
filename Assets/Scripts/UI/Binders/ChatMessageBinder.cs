using System;
using R3;
using UI.Core;
using UI.Services.Data;
using UnityEngine;

namespace UI.Binders
{
    [Serializable]
    public class ChatMessageBinder : ViewBinder<ReactiveCommand<ChatMessageDataView>>
    {
        [SerializeField] private RectTransform _content;

        private IDisposable _subscription;

        public override void Parse(ReactiveCommand<ChatMessageDataView> command)
        {
            _subscription?.Dispose();
            _subscription = command.Subscribe(SpawnMessage);
        }

        private void SpawnMessage(ChatMessageDataView data)
        {
            data.InstantiateView.transform.SetParent(_content, false);
            data.InstantiateView.transform.SetAsFirstSibling();

            data.InstantiateView.SetMessage(FormatMessage(data.Data));
            data.InstantiateView.SetColor(data.Data.Color);
        }

        private static string FormatMessage(ChatMessageData data)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            var type = GetTypeLabel(data.Type);
            var username = FormatText(string.IsNullOrWhiteSpace(data.Username) ? "System" : data.Username);
            var message = FormatText(data.Text);

            return $"<size=80%>[{time}] {type}</size> <b>{username}</b>: {message}";
        }

        private static string GetTypeLabel(ChatMessageType type)
        {
            return type switch
            {
                ChatMessageType.Warning => "[WARN]",
                ChatMessageType.Error => "[ERROR]",
                _ => "[INFO]"
            };
        }

        private static string FormatText(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim()
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
        }

        public override void Dispose()
        {
            base.Dispose();
            _subscription?.Dispose();
        }
    }
}
