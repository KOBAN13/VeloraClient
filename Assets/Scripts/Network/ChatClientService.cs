using System;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;
using UI.Services;
using UI.Services.Data;
using UnityEngine;

namespace Network
{
    public class ChatClientService : IChatClientService, IInitializable, IDisposable
    {
        private readonly INetworkClient _networkClient;
        private readonly IChatService _chatService;

        private readonly CompositeDisposable _disposables = new();

        public bool IsInitialized { get; set; }

        public ChatClientService(INetworkClient networkClient, IChatService chatService)
        {
            _networkClient = networkClient;
            _chatService = chatService;
        }

        public void Initialize()
        {
            _networkClient.Received
                .Where(packet => packet.MsgCase == Packet.MsgOneofCase.Chat)
                .Subscribe(ReceiveMessage)
                .AddTo(_disposables);
        }

        public void SendMessage(string msg)
        {
            var packet = new Packet
            {
                Chat = new ChatMessage
                {
                    Msg = msg
                }
            };

            _networkClient.SendAsync(packet).Forget();
        }

        private void ReceiveMessage(Packet packet)
        {
            _chatService.AddMessage(new ChatMessageData($"Client: {packet.SenderId}", packet.Chat.Msg, Color.white));
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
