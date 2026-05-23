using System;
using Core.Utils.Services;
using Network.Contracts;
using Network.Messaging;
using Packets;
using R3;
using UI.Services;
using UI.Services.Data;
using UnityEngine;

namespace Network.Services.Chat
{
    public class ChatClientService : IChatClientService, IInitializable, IDisposable
    {
        private readonly INetworkMessageBus _networkMessageBus;
        private readonly IChatService _chatService;

        private readonly CompositeDisposable _disposables = new();

        public bool IsInitialized { get; set; }

        public ChatClientService(IChatService chatService, INetworkMessageBus networkMessageBus)
        {
            _chatService = chatService;
            _networkMessageBus = networkMessageBus;
        }

        public void Initialize()
        {
            _networkMessageBus
                .On<ChatMessage>()
                .Subscribe(ReceiveMessage)
                .AddTo(_disposables);
        }

        public void SendMessage(string msg)
        {
            _networkMessageBus.Send(new ChatMessage
            {
                Msg = msg
            });
        }

        private void ReceiveMessage(NetworkMessage<ChatMessage> message)
        {
            _chatService.AddMessage(new ChatMessageData($"Client: {message.SenderId}", message.Payload.Msg, Color.white));
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
