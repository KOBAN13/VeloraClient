using System;
using System.Collections.Generic;
using Core.Utils.Services;
using Network.Transport.Data;
using R3;

namespace Network
{
    public class RoomStateService : IRoomStateService, IInitializable, IDisposable
    {
        private readonly ILobbyClientService _lobbyClientService;
        private readonly CompositeDisposable _disposables = new();

        public Observable<IReadOnlyList<RoomSummaryData>> RoomsChanged { get; }
        public Observable<RoomStateData> CurrentRoomChanged { get; }
        public Observable<string> ErrorReceived { get; }
        public IReadOnlyList<RoomSummaryData> Rooms { get; }
        public RoomStateData CurrentRoom { get; }
        public PlayerData LocalPLayer { get; }
        public bool IsInRoom { get; }
        public bool IsOwner { get; }
        public bool CanToggleReady { get; }
        public bool CanStart { get; }
        
        public bool IsInitialized { get; set; }
        
        
        public RoomStateService(ILobbyClientService lobbyClientService)
        {
            _lobbyClientService = lobbyClientService;
        }
        
        
        public void Initialize()
        {
            _lobbyClientService.RoomListSnapshotReceived
                .Subscribe()
                .AddTo(_disposables);
            
            _lobbyClientService.RoomStateSnapshotReceived
                .Subscribe()
                .AddTo(_disposables);
            
            _lobbyClientService.LobbyErrorReceived
                .Subscribe()
                .AddTo(_disposables);
        }
        
        public void RefreshRooms()
        {
            
        }

        public void CreateRoom(uint maxPlayers)
        {
            
        }

        public void JoinRoom(ulong roomId)
        {
            
        }

        public void LeaveRoom()
        {
            
        }

        public void SetReady(bool isReady)
        {
            
        }

        public void StartGame()
        {
            
        }

        public void ClearCurrentRoom()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}