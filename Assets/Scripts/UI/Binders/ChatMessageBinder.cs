using System;
using System.Collections.Generic;
using Core.Utils.Extensions;
using Core.Utils.Pool;
using R3;
using UI.Core;
using UI.Services.Data;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Binders
{
    [Serializable]
    public class ChatMessageBinder : ViewBinder<ChatMessageFeed>
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private ScrollRect _scrollRect;

        private readonly List<MessageView> _spawnedMessages = new();
        private IDisposable _subscription;
        private IPoolService _poolService;

        public void SetPoolService(IPoolService poolService)
        {
            _poolService = poolService;
        }

        public override void Parse(ChatMessageFeed feed)
        {
            _subscription?.Dispose();
            ClearSpawnedMessages();
            
            foreach (var message in feed.messages)
                SpawnMessage(message);

            _subscription = feed.onMessageAdded.Subscribe(SpawnMessage);
        }

        private void SpawnMessage(ChatMessageData data)
        {
            var messageView = _poolService.Spawn<MessageView>(EObjectInPoolName.ChatMessage, false);

            _spawnedMessages.Add(messageView);

            messageView.transform.SetParent(_content, false);
            messageView.SetMessage(data.FormatMessage());
            messageView.SetColor(data.Color);
            messageView.gameObject.SetActive(true);

            messageView.transform.SetAsLastSibling();
        }

        public override void Dispose()
        {
            base.Dispose();
            _subscription?.Dispose();
            ClearSpawnedMessages();
        }

        private void ClearSpawnedMessages()
        {
            foreach (var message in _spawnedMessages)
            {
                _poolService.ReturnToPool(EObjectInPoolName.ChatMessage, message);
            }

            _spawnedMessages.Clear();
        }
    }
}
