using System;
using System.Linq;
using Core.Utils.Logger;
using Core.Utils.Services;
using Network.Transport.Data;
using ObservableCollections;
using Packets;
using R3;
using UnityEngine;

namespace Network
{
    public class RoomStateService : IRoomStateService, IInitializable, IDisposable
    {
        private readonly ILobbyClientService _lobbyClientService;
        private readonly IClientIdentityService _clientIdentityService;
        private readonly ILoggerService _loggerService;
        private readonly CompositeDisposable _disposables = new();

        private readonly ObservableList<RoomSummaryData> _roomSummaryData = new();
        private readonly Subject<RoomStateData> _roomStateSubject = new();
        private readonly Subject<string> _errorReceived = new();

        public Observable<RoomStateData> CurrentRoomChanged => _roomStateSubject;
        public Observable<string> ErrorReceived => _errorReceived;
        public IReadOnlyObservableList<RoomSummaryData> RoomSummaryData => _roomSummaryData;

        public RoomStateData CurrentRoom { get; private set; }
        public PlayerData LocalPLayer { get; private set; }
        public bool IsInRoom { get; private set; }
        public bool IsOwner { get; private set; }
        public bool CanToggleReady { get; private set; }
        public bool CanStart { get; private set; }

        public bool IsInitialized { get; set; }


        public RoomStateService(ILobbyClientService lobbyClientService, IClientIdentityService clientIdentityService, ILoggerService loggerService)
        {
            _lobbyClientService = lobbyClientService;
            _clientIdentityService = clientIdentityService;
            _loggerService = loggerService;
        }


        public void Initialize()
        {
            _lobbyClientService.RoomListSnapshotReceived
                .Subscribe(OnUpdateRoomList)
                .AddTo(_disposables);

            _lobbyClientService.RoomStateSnapshotReceived
                .Subscribe(OnUpdateRoomState)
                .AddTo(_disposables);

            _lobbyClientService.LobbyErrorReceived
                .Subscribe(OnLobbyError)
                .AddTo(_disposables);
        }
        
        public void RefreshRooms()
        {
            _lobbyClientService.RefreshRooms();
        }

        public void CreateRoom(string roomName, uint maxPlayers)
        {
            _lobbyClientService.CreateRoom(roomName, maxPlayers);
        }

        public void JoinRoom(ulong roomId)
        {
            _lobbyClientService.JoinRoom(roomId);
        }

        public void LeaveRoom()
        {
            _lobbyClientService.LeaveRoom();
        }

        public void SetReady(bool isReady)
        {
            _lobbyClientService.SetReady(isReady);
        }

        public void StartGame()
        {
            _lobbyClientService.StartGame();
        }

        public void ClearCurrentRoom()
        {
            CurrentRoom = null;
            LocalPLayer = default;
            IsInRoom = false;
            IsOwner = false;
            CanToggleReady = false;
            CanStart = false;
            
            //TODO Если нужно вообщить об отчистке
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        private void OnLobbyError(string error)
        {
            Debug.LogError(error);
            _errorReceived.OnNext(error);
        }

        private void OnUpdateRoomList(RoomListSnapshotMessage roomListSnapshotMessage)
        {
            var rooms = roomListSnapshotMessage.Rooms;
            var roomsCount = rooms.Count;

            if (roomsCount == 0)
            {
                if (_roomSummaryData.Count > 0)
                {
                    _roomSummaryData.Clear();
                }

                return;
            }

            for (var targetIndex = 0; targetIndex < roomsCount; targetIndex++)
            {
                var room = rooms[targetIndex];
                var roomSummaryData = new RoomSummaryData(
                    room.RoomId,
                    room.PlayersCount,
                    room.MaxPlayer,
                    room.Status);

                if (targetIndex >= _roomSummaryData.Count)
                {
                    _roomSummaryData.Add(roomSummaryData);
                    continue;
                }

                var current = _roomSummaryData[targetIndex];
                if (current.RoomId == roomSummaryData.RoomId)
                {
                    if (current != roomSummaryData)
                    {
                        _roomSummaryData[targetIndex] = roomSummaryData;
                    }

                    continue;
                }

                var existingIndex = FindRoomIndex(roomSummaryData.RoomId, targetIndex + 1);
                if (existingIndex >= 0)
                {
                    _roomSummaryData.Move(existingIndex, targetIndex);

                    if (!_roomSummaryData[targetIndex].Equals(roomSummaryData))
                    {
                        _roomSummaryData[targetIndex] = roomSummaryData;
                    }
                }
                else
                {
                    _roomSummaryData.Insert(targetIndex, roomSummaryData);
                }
            }

            if (_roomSummaryData.Count > roomsCount)
            {
                _roomSummaryData.RemoveRange(roomsCount, _roomSummaryData.Count - roomsCount);
            }
        }

        private void OnUpdateRoomState(RoomStateSnapshotMessage roomStateSnapshotMessage)
        {
            var roomPlayers = roomStateSnapshotMessage.Player;
            var playersCount = roomPlayers.Count;
            var players = playersCount == 0 ? Array.Empty<PlayerData>() : new PlayerData[playersCount];

            for (var i = 0; i < playersCount; i++)
            {
                var roomPlayerMessage = roomPlayers[i];
                var player = new PlayerData(roomPlayerMessage.UserId, roomPlayerMessage.ClientId,
                    roomPlayerMessage.Username, roomPlayerMessage.IsReady, roomPlayerMessage.Owner);

                players[i] = player;
            }

            var roomStateData = new RoomStateData(
                roomStateSnapshotMessage.RoomId,
                roomStateSnapshotMessage.MaxPlayer,
                roomStateSnapshotMessage.Status,
                players);

            CurrentRoom = roomStateData;
            IsInRoom = true;
            LocalPLayer = FindLocalPlayer(players);
            IsOwner = LocalPLayer.IsOwner;
            CanToggleReady = IsInRoom && roomStateSnapshotMessage.Status == RoomStatus.Waiting;
            CanStart = CanStartRoom(roomStateSnapshotMessage.Status, players);

            _roomStateSubject.OnNext(roomStateData);
        }

        private PlayerData FindLocalPlayer(PlayerData[] players)
        {
            var clientId = _clientIdentityService.ClientId;
            
            if (clientId == 0)
            {
                return default;
            }

            foreach (var player in players)
            {
                if (player.ClientId == clientId || player.UserId == clientId)
                {
                    return player;
                }
            }

            return default;
        }

        private bool CanStartRoom(RoomStatus status, PlayerData[] players)
        {
            if (!IsOwner || status != RoomStatus.Waiting || players.Length <= 1 || LocalPLayer == default)
            {
                return false;
            }

            return players
                .All(player => player.IsOwner || player.IsReady);
        }

        private int FindRoomIndex(ulong roomId, int startIndex)
        {
            for (var i = startIndex; i < _roomSummaryData.Count; i++)
            {
                if (_roomSummaryData[i].RoomId == roomId)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
