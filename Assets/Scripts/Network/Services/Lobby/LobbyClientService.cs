using System;
using System.Collections.Generic;
using Core.Utils.Services;
using Network.Contracts;
using Network.Data;
using Network.Messaging;
using ObservableCollections;
using Packets;
using R3;

namespace Network.Services.Lobby
{
    public class LobbyClientService : ILobbyClientService, IInitializable, IDisposable
    {
        private readonly INetworkMessageBus _messages;
        private readonly IClientIdentityService _clientIdentityService;
        
        private readonly Subject<RoomStateSnapshotMessage> _roomStateSnapshotReceived = new();
        private readonly Subject<RoomListSnapshotMessage> _roomListSnapshotReceived = new();
        private readonly ObservableList<PlayerData> _players = new();
        private readonly Subject<string> _lobbyErrorRequest = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly Subject<Unit> _kickedUser = new();

        public bool IsInitialized { get; set; }

        public Observable<RoomListSnapshotMessage> RoomListSnapshotReceived => _roomListSnapshotReceived;
        public Observable<RoomStateSnapshotMessage> RoomStateSnapshotReceived => _roomStateSnapshotReceived;
        public Observable<string> LobbyErrorReceived => _lobbyErrorRequest;
        public IReadOnlyObservableList<PlayerData> Players => _players;
        public Observable<Unit> KickedUser => _kickedUser;

        public LobbyClientService(INetworkMessageBus messages, IClientIdentityService clientIdentityService)
        {
            _messages = messages;
            _clientIdentityService = clientIdentityService;
        }
        
        public void Initialize()
        {
            _messages.On<DenyResponseMessage>()
                .Subscribe(message => _lobbyErrorRequest.OnNext(message.Payload.Reason))
                .AddTo(_disposables);
            
            _messages.On<RoomStateSnapshotMessage>()
                .Subscribe(message => OnRoomStateSnapshotReceived(message.Payload))
                .AddTo(_disposables);

            _messages.On<RoomListSnapshotMessage>()
                .Subscribe(message => _roomListSnapshotReceived.OnNext(message.Payload))
                .AddTo(_disposables);
            
            _messages.On<PlayerJoinRoom>()
                .Subscribe(message => OnPlayerJoined(message.Payload.Player))
                .AddTo(_disposables);
            
            _messages.On<PlayerRemoveRoom>()
                .Subscribe(message => OnPlayerRemoved(message.Payload.Player))
                .AddTo(_disposables);
            
            _messages.On<PlayerKickRoom>()
                .Subscribe(message => OnPlayerKicked(message.Payload))
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
            _players.Clear();
            _messages.Send(new CreateRoomRequestMessage()
            {
                MaxPlayer = maxPlayers,
                RoomName = nameRoom
            });
        }

        public void LeaveRoom()
        {
            _players.Clear();
            _messages.Send(new LeaveRoomRequestMessage()
            {
            });
        }
        
        public void JoinRoom(ulong roomId)
        {
            _players.Clear();
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

        public void KickUser(ulong userId)
        {
            _messages.Send(new PlayerKickRoom()
            {
                UserId = userId
            });
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnRoomStateSnapshotReceived(RoomStateSnapshotMessage roomStateSnapshotMessage)
        {
            ReconcilePlayers(roomStateSnapshotMessage.Player);
            _roomStateSnapshotReceived.OnNext(roomStateSnapshotMessage);
        }

        private void OnPlayerJoined(RoomPlayerMessage roomPlayerMessage)
        {
            UpsertPlayer(CreatePlayerData(roomPlayerMessage));
        }

        private void OnPlayerRemoved(RoomPlayerMessage roomPlayerMessage)
        {
            RemovePlayerByUserId(roomPlayerMessage.UserId);
        }

        private void OnPlayerKicked(PlayerKickRoom playerKickRoom)
        {
            var isLocalPlayer = IsLocalPlayer(playerKickRoom.UserId);

            RemovePlayerByUserId(playerKickRoom.UserId);

            if (isLocalPlayer)
            {
                _kickedUser.OnNext(Unit.Default);
            }
        }

        private void ReconcilePlayers(IEnumerable<RoomPlayerMessage> roomPlayers)
        {
            var roomPlayerUserIds = new HashSet<ulong>();

            foreach (var roomPlayerMessage in roomPlayers)
            {
                var playerData = CreatePlayerData(roomPlayerMessage);
                roomPlayerUserIds.Add(playerData.UserId);
                UpsertPlayer(playerData);
            }

            if (roomPlayerUserIds.Count == 0)
            {
                if (_players.Count > 0)
                {
                    _players.Clear();
                }

                return;
            }

            List<PlayerData> stalePlayers = null;

            foreach (var player in _players)
            {
                if (roomPlayerUserIds.Contains(player.UserId))
                {
                    continue;
                }

                stalePlayers ??= new List<PlayerData>();
                stalePlayers.Add(player);
            }

            if (stalePlayers == null)
            {
                return;
            }

            foreach (var stalePlayer in stalePlayers)
            {
                _players.Remove(stalePlayer);
            }
        }

        private void UpsertPlayer(PlayerData playerData)
        {
            var playerIndex = FindPlayerIndex(playerData.UserId);

            if (playerIndex < 0)
            {
                _players.Add(playerData);
                return;
            }

            if (_players[playerIndex] != playerData)
            {
                _players[playerIndex] = playerData;
            }
        }

        private void RemovePlayerByUserId(ulong userId)
        {
            var playerIndex = FindPlayerIndex(userId);

            if (playerIndex < 0)
            {
                return;
            }

            _players.Remove(_players[playerIndex]);
        }

        private int FindPlayerIndex(ulong userId)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].UserId == userId)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool IsLocalPlayer(ulong userId)
        {
            var playerIndex = FindPlayerIndex(userId);

            if (playerIndex < 0)
            {
                return false;
            }

            return _players[playerIndex].ClientId == _clientIdentityService.ClientId;
        }

        private static PlayerData CreatePlayerData(RoomPlayerMessage roomPlayerMessage)
        {
            return new PlayerData(
                roomPlayerMessage.UserId,
                roomPlayerMessage.ClientId,
                roomPlayerMessage.Username,
                roomPlayerMessage.IsReady,
                roomPlayerMessage.Owner);
        }
    }
}
