using System;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;
using UnityEngine;

namespace Network
{
    public class RegisterClientService : IRegisterClientService, IInitializable, IDisposable
    {
        public bool IsInitialized { get; set; }

        public Observable<Unit> SuccessRegister => _successRegister;
        public Observable<string> RegisterErrorRequest => _registerErrorRequest;
        
        private readonly INetworkClient _networkClient;
        
        private readonly Subject<Unit> _successRegister = new();
        private readonly Subject<string> _registerErrorRequest = new();
        private readonly CompositeDisposable _disposables = new();
        
        public RegisterClientService(INetworkClient networkClient)
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

        public void Register(string username, string password)
        {
            var packet = new Packet()
            {
                RegisterRequest = new RegisterRequestMessage()
                {
                    Username = username,
                    Password = password
                }
            };
            
            _networkClient.SendAsync(packet).Forget();
        }
        
        private void ReceiveMessage(Packet packet)
        {
            Debug.LogError("Прикол");
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}