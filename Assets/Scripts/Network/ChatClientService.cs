using System;
using Core.Utils.Logger;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;

namespace Network
{
    public class ChatClientService : IChatClientService, IInitializable, IDisposable
    {
        private readonly INetworkClient _networkClient;
        private readonly ILoggerService _loggerService;

        private readonly CompositeDisposable _disposables = new();

        public bool IsInitialized { get; set; }

        public ChatClientService(INetworkClient networkClient, ILoggerService loggerService)
        {
            _networkClient = networkClient;
            _loggerService = loggerService;
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
            _loggerService.Log(packet.Chat.Msg, $"Client: {packet.SenderId}");
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}