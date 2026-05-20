using System;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;
using UnityEngine;

namespace Network
{
    public class LobbyClientService : ILobbyClientService, IInitializable, IDisposable
    {
        private readonly INetworkClient _networkClient;
        
        private readonly Subject<RoomStateSnapshotMessage> _roomStateSnapshotReceived = new();
        private readonly Subject<RoomListSnapshotMessage> _roomListSnapshotReceived = new();
        private readonly Subject<string> _lobbyErrorRequest = new();
        private readonly CompositeDisposable _disposables = new();

        public bool IsInitialized { get; set; }

        public Observable<RoomListSnapshotMessage> RoomListSnapshotReceived => _roomListSnapshotReceived;
        public Observable<RoomStateSnapshotMessage> RoomStateSnapshotReceived => _roomStateSnapshotReceived;
        public Observable<string> LobbyErrorReceived => _lobbyErrorRequest;
        
        public LobbyClientService(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }
        
        public void Initialize()
        {
            _networkClient.Received
                .Where(packets => packets.MsgCase is Packet.MsgOneofCase.DenyResponse or Packet.MsgOneofCase.RoomStateSnapshot)
                .Subscribe(ReceiveMessage)
                .AddTo(_disposables);
            
            _networkClient.Received.
                Where(packets => packets.MsgCase is Packet.MsgOneofCase.RoomListSnapshot)
                .Subscribe(packet => _roomListSnapshotReceived.OnNext(packet.RoomListSnapshot))
                .AddTo(_disposables);
        }
        
        public void RefreshRooms()
        {
            var packet = new Packet()
            {
                RoomListRequest = new RoomListRequestMessage()
                {

                }
            };
            
            _networkClient.SendAsync(packet);
        }

        public void CreateRoom(string nameRoom, uint maxPlayers)
        {
            var packet = new Packet()
            {
                CreateRoomRequest = new CreateRoomRequestMessage()
                {
                    MaxPlayer =  maxPlayers,
                    RoomName = nameRoom
                }
            };
            
            _networkClient.SendAsync(packet);
        }

        public void LeaveRoom()
        {
            var packet = new Packet()
            {
                LeaveRoomRequest = new LeaveRoomRequestMessage()
                {
                    
                }
            };
            
            _networkClient.SendAsync(packet);
        }
        
        public void JoinRoom(ulong roomId)
        {
            var packet = new Packet()
            {
                JoinRoomRequest = new JoinRoomRequestMessage()
                {
                    RoomId = roomId
                }
            };
            
            _networkClient.SendAsync(packet);
        }
        
        public void SetReady(bool isReady)
        {
            var packet = new Packet()
            {
                ReadyRequest = new ReadyRequestMessage()
                {
                    IsReady = isReady
                }
            };
            
            _networkClient.SendAsync(packet);
        }

        public void StartGame()
        {
            var packet = new Packet()
            {
                StartGame = new StartGameRequestMessage()
                {

                }
            };
            
            _networkClient.SendAsync(packet);
        }
        
        private void ReceiveMessage(Packet packet)
        {
            switch (packet.MsgCase)
            {
                case Packet.MsgOneofCase.DenyResponse:
                    _lobbyErrorRequest.OnNext(packet.DenyResponse.Reason);
                    break;
                case Packet.MsgOneofCase.RoomStateSnapshot:
                    _roomStateSnapshotReceived.OnNext(packet.RoomStateSnapshot);
                    break;
            }
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}