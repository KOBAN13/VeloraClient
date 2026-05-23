using System;
using Core.Utils.Services;
using Network.Contracts;
using Network.Messaging;
using Packets;
using R3;

namespace Network.Services.Lobby
{
    public class LobbyClientService : ILobbyClientService, IInitializable, IDisposable
    {
        private readonly INetworkMessageBus _messages;
        
        private readonly Subject<RoomStateSnapshotMessage> _roomStateSnapshotReceived = new();
        private readonly Subject<RoomListSnapshotMessage> _roomListSnapshotReceived = new();
        private readonly Subject<string> _lobbyErrorRequest = new();
        private readonly CompositeDisposable _disposables = new();

        public bool IsInitialized { get; set; }

        public Observable<RoomListSnapshotMessage> RoomListSnapshotReceived => _roomListSnapshotReceived;
        public Observable<RoomStateSnapshotMessage> RoomStateSnapshotReceived => _roomStateSnapshotReceived;
        public Observable<string> LobbyErrorReceived => _lobbyErrorRequest;
        
        public LobbyClientService(INetworkMessageBus messages)
        {
            _messages = messages;
        }
        
        public void Initialize()
        {
            _messages.On<DenyResponseMessage>()
                .Subscribe(message => _lobbyErrorRequest.OnNext(message.Payload.Reason))
                .AddTo(_disposables);
            
            _messages.On<RoomStateSnapshotMessage>()
                .Subscribe(message => _roomStateSnapshotReceived.OnNext(message.Payload))
                .AddTo(_disposables);

            _messages.On<RoomListSnapshotMessage>()
                .Subscribe(message => _roomListSnapshotReceived.OnNext(message.Payload))
                .AddTo(_disposables);
        }
        
        public void RefreshRooms()
        {
            _messages.Send(new RoomListRequestMessage()
            {
            });
        }

        public void CreateRoom(string nameRoom, uint maxPlayers)
        {
            _messages.Send(new CreateRoomRequestMessage()
            {
                MaxPlayer = maxPlayers,
                RoomName = nameRoom
            });
        }

        public void LeaveRoom()
        {
            _messages.Send(new LeaveRoomRequestMessage()
            {
            });
        }
        
        public void JoinRoom(ulong roomId)
        {
            _messages.Send(new JoinRoomRequestMessage()
            {
                RoomId = roomId
            });
        }
        
        public void SetReady(bool isReady)
        {
            _messages.Send(new ReadyRequestMessage()
            {
                IsReady = isReady
            });
        }

        public void StartGame()
        {
            _messages.Send(new StartGameRequestMessage()
            {
            });
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
