using System;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;

namespace Network
{
    public class LoginClientService : ILoginClientService, IInitializable, IDisposable
    {
        public bool IsInitialized { get; set; }
        
        private readonly INetworkClient _networkClient;
        
        private readonly Subject<Unit> _successLogin = new();
        private readonly Subject<string> _loginErrorRequest = new();
        private readonly CompositeDisposable _disposables = new();
        
        public Observable<Unit> SuccessLogin => _successLogin;
        public Observable<string> LoginErrorRequest => _loginErrorRequest;

        public LoginClientService(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }

        public void Initialize()
        {
            _networkClient.Received
                .Where(packet => packet.MsgCase is Packet.MsgOneofCase.OkResponse or Packet.MsgOneofCase.DenyResponse)
                .Subscribe(ReceiveMessage)
                .AddTo(_disposables);
        }

        public void Login(string username, string password)
        {
            var packet = new Packet
            {
                LoginRequest = new LoginRequestMessage
                {
                    Username = username,
                    Password = password
                }
            };
            
            _networkClient.SendAsync(packet).Forget();
        }
        
        private void ReceiveMessage(Packet packet)
        {
            switch (packet.MsgCase)
            {
                case Packet.MsgOneofCase.OkResponse:
                    _successLogin.OnNext(Unit.Default);
                    break;
                case Packet.MsgOneofCase.DenyResponse:
                    _loginErrorRequest.OnNext(packet.DenyResponse.Reason);
                    break;
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
