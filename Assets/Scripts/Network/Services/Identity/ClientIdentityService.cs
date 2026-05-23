using System;
using Core.Utils.Services;
using Network.Contracts;
using Network.Messaging;
using Packets;
using R3;

namespace Network.Services.Identity
{
    public class ClientIdentityService : IClientIdentityService, IInitializable, IDisposable
    {
        private readonly INetworkMessageBus _messages;
        private readonly CompositeDisposable _disposables = new();
        private readonly Subject<ulong> _clientIdChanged = new();

        public ulong ClientId { get; private set; }

        public Observable<ulong> ClientIdChanged => _clientIdChanged;
        
        public bool IsInitialized { get; set; }

        public ClientIdentityService(INetworkMessageBus messages)
        {
            _messages = messages;
        }

        public void Initialize()
        {
            _messages.On<IdMessage>()
                .Subscribe(OnIdReceived)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnIdReceived(NetworkMessage<IdMessage> message)
        {
            var clientId = message.Payload.Id;
            
            if (ClientId == clientId)
            {
                return;
            }

            ClientId = clientId;
            _clientIdChanged.OnNext(ClientId);
        }
    }
}
