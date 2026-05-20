using System;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;

namespace Network
{
    public class ClientIdentityService : IClientIdentityService, IInitializable, IDisposable
    {
        private readonly INetworkClient _networkClient;
        private readonly CompositeDisposable _disposables = new();
        private readonly Subject<ulong> _clientIdChanged = new();

        public ulong ClientId { get; private set; }

        public Observable<ulong> ClientIdChanged => _clientIdChanged;
        
        public bool IsInitialized { get; set; }

        public ClientIdentityService(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }

        public void Initialize()
        {
            _networkClient.Received
                .Where(packet => packet.MsgCase == Packet.MsgOneofCase.Id)
                .Subscribe(OnIdReceived)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnIdReceived(Packet packet)
        {
            var clientId = packet.Id.Id;
            
            if (ClientId == clientId)
            {
                return;
            }

            ClientId = clientId;
            _clientIdChanged.OnNext(ClientId);
        }
    }
}
