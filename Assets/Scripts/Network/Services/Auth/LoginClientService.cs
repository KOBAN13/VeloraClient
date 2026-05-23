using System;
using Core.Utils.Services;
using Network.Contracts;
using Network.Messaging;
using Packets;
using R3;

namespace Network.Services.Auth
{
    public class LoginClientService : ILoginClientService, IInitializable, IDisposable
    {
        public bool IsInitialized { get; set; }
        
        private readonly INetworkMessageBus _messages;
        
        private readonly Subject<Unit> _successLogin = new();
        private readonly Subject<string> _loginErrorRequest = new();
        private readonly CompositeDisposable _disposables = new();
        
        public Observable<Unit> SuccessLogin => _successLogin;
        public Observable<string> LoginErrorRequest => _loginErrorRequest;

        public LoginClientService(INetworkMessageBus messages)
        {
            _messages = messages;
        }

        public void Initialize()
        {
            _messages.On<OkResponseMessage>()
                .Subscribe(_ => _successLogin.OnNext(Unit.Default))
                .AddTo(_disposables);

            _messages.On<DenyResponseMessage>()
                .Subscribe(message => _loginErrorRequest.OnNext(message.Payload.Reason))
                .AddTo(_disposables);
        }

        public void Login(string username, string password)
        {
            _messages.Send(new LoginRequestMessage
            {
                Username = username,
                Password = password
            });
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
