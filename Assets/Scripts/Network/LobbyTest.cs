using System;
using System.Text;
using Core.Utils.Services;
using Network.Transport;
using Packets;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Network
{
    public class LobbyTest : MonoBehaviour
    {
        private const string LogPrefix = "[LobbyTest]";

        [Header("Create Room")]
        [SerializeField] private string _roomName = "test_game";
        [SerializeField, MinValue(1)] private uint _maxPlayers = 4;

        [Header("Join Room")]
        [SerializeField] private ulong _roomId = 1;

        [Header("Ready")]
        [SerializeField] private bool _isReady = true;

        [Header("Debug")]
        [SerializeField] private bool _refreshRoomsOnStart;

        private readonly CompositeDisposable _disposables = new();

        private ILobbyClientService _lobbyClientService;
        private INetworkClient _networkClient;
        private RoomListSnapshotMessage _lastRoomList;
        private RoomStateSnapshotMessage _lastRoomState;
        private bool _isSubscribed;

        [Inject]
        private void Construct(ILobbyClientService lobbyClientService, INetworkClient networkClient)
        {
            _lobbyClientService = lobbyClientService;
            _networkClient = networkClient;

            EnsureInitialized(_networkClient, nameof(INetworkClient));
            EnsureInitialized(_lobbyClientService, nameof(ILobbyClientService));
            Subscribe();
        }

        private void Start()
        {
            Log("Lobby test is ready.");

            if (_refreshRoomsOnStart)
            {
                RefreshRooms();
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        [Button]
        private void RefreshRooms()
        {
            RunLobbyCommand("Refresh room list", () => _lobbyClientService.RefreshRooms());
        }

        [Button]
        private void CreateGame()
        {
            if (string.IsNullOrWhiteSpace(_roomName))
            {
                LogError("Create room failed.\nReason: room name is empty.");
                return;
            }

            if (_maxPlayers == 0)
            {
                LogError("Create room failed.\nReason: max players must be greater than 0.");
                return;
            }

            RunLobbyCommand(
                $"Create room \"{_roomName}\" with max players {_maxPlayers}",
                () => _lobbyClientService.CreateRoom(_roomName.Trim(), _maxPlayers));
        }

        [Button]
        private void JoinGame()
        {
            if (_roomId == 0)
            {
                LogError("Join room failed.\nReason: room id must be greater than 0.");
                return;
            }

            RunLobbyCommand($"Join room #{_roomId}", () => _lobbyClientService.JoinRoom(_roomId));
        }

        [Button]
        private void JoinFirstAvailableRoom()
        {
            if (_lastRoomList == null || _lastRoomList.Rooms.Count == 0)
            {
                LogError("Join first available room failed.\nReason: room list is empty. Press RefreshRooms first.");
                return;
            }

            var room = _lastRoomList.Rooms[0];
            _roomId = room.RoomId;

            RunLobbyCommand($"Join first available room #{_roomId}", () => _lobbyClientService.JoinRoom(_roomId));
        }

        [Button]
        private void LeaveGame()
        {
            RunLobbyCommand("Leave current room", () => _lobbyClientService.LeaveRoom());
        }

        [Button]
        private void SetReady()
        {
            RunLobbyCommand($"Set ready = {_isReady}", () => _lobbyClientService.SetReady(_isReady));
        }

        [Button]
        private void SetReadyTrue()
        {
            _isReady = true;
            SetReady();
        }

        [Button]
        private void SetReadyFalse()
        {
            _isReady = false;
            SetReady();
        }

        [Button]
        private void StartGame()
        {
            RunLobbyCommand("Start game", () => _lobbyClientService.StartGame());
        }

        [Button]
        private void PrintCachedLobbyState()
        {
            if (_lastRoomList == null && _lastRoomState == null)
            {
                Log("Cached lobby state is empty.");
                return;
            }

            if (_lastRoomList != null)
            {
                LogRoomList(_lastRoomList);
            }

            if (_lastRoomState != null)
            {
                LogRoomState(_lastRoomState);
            }
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            _lobbyClientService.RoomListSnapshotReceived
                .Subscribe(OnRoomListSnapshotReceived)
                .AddTo(_disposables);

            _lobbyClientService.RoomStateSnapshotReceived
                .Subscribe(OnRoomStateSnapshotReceived)
                .AddTo(_disposables);

            _lobbyClientService.LobbyErrorReceived
                .Subscribe(OnLobbyErrorReceived)
                .AddTo(_disposables);

            _networkClient.Connected
                .Subscribe(_ => Log("Network connected."))
                .AddTo(_disposables);

            _networkClient.Disconnected
                .Subscribe(disconnected => LogWarning(
                    $"Network disconnected.\nCode: {disconnected.Code}\nReason: {disconnected.Reason}"))
                .AddTo(_disposables);

            _networkClient.Error
                .Subscribe(error => LogError($"Network error.\nReason: {error}"))
                .AddTo(_disposables);

            _networkClient.Received
                .Where(packet => packet.MsgCase == Packet.MsgOneofCase.MatchStarted)
                .Subscribe(OnMatchStartedReceived)
                .AddTo(_disposables);

            _isSubscribed = true;
        }

        private void RunLobbyCommand(string action, Action command)
        {
            if (_lobbyClientService == null)
            {
                LogError($"{action} failed.\nReason: {nameof(ILobbyClientService)} is not injected.");
                return;
            }

            try
            {
                Log($"Command sent.\nAction: {action}");
                command.Invoke();
            }
            catch (Exception e)
            {
                LogError($"{action} failed.\nException: {e.Message}");
            }
        }

        private void OnRoomListSnapshotReceived(RoomListSnapshotMessage roomList)
        {
            _lastRoomList = roomList;
            LogRoomList(roomList);
        }

        private void OnRoomStateSnapshotReceived(RoomStateSnapshotMessage roomState)
        {
            _lastRoomState = roomState;
            _roomId = roomState.RoomId;

            LogRoomState(roomState);
        }

        private void OnLobbyErrorReceived(string error)
        {
            LogError($"Lobby error.\nReason: {error}");
        }

        private void OnMatchStartedReceived(Packet packet)
        {
            var match = packet.MatchStarted;

            Log(
                "Match started.\n" +
                $"Room: #{match.RoomId}\n" +
                $"Match: #{match.MatchId}");
        }

        private void LogRoomList(RoomListSnapshotMessage roomList)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Room list snapshot.");
            builder.AppendLine($"Rooms: {roomList.Rooms.Count}");

            if (roomList.Rooms.Count == 0)
            {
                builder.AppendLine("  (empty)");
                Log(builder.ToString());
                return;
            }

            for (var i = 0; i < roomList.Rooms.Count; i++)
            {
                var room = roomList.Rooms[i];
                builder
                    .Append("  ")
                    .Append(i + 1)
                    .Append(". #")
                    .Append(room.RoomId)
                    .Append(" \"")
                    .Append(room.Name)
                    .Append("\" players=")
                    .Append(room.PlayersCount)
                    .Append("/")
                    .Append(room.MaxPlayer)
                    .Append(" status=")
                    .Append(room.Status)
                    .AppendLine();
            }

            Log(builder.ToString());
        }

        private void LogRoomState(RoomStateSnapshotMessage roomState)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Room state snapshot.");
            builder
                .Append("Room: #")
                .Append(roomState.RoomId)
                .Append(" status=")
                .Append(roomState.Status)
                .Append(" players=")
                .Append(roomState.Player.Count)
                .Append("/")
                .Append(roomState.MaxPlayer)
                .AppendLine();

            if (roomState.Player.Count == 0)
            {
                builder.AppendLine("Players: (empty)");
                Log(builder.ToString());
                return;
            }

            builder.AppendLine("Players:");

            for (var i = 0; i < roomState.Player.Count; i++)
            {
                var player = roomState.Player[i];
                builder
                    .Append("  ")
                    .Append(i + 1)
                    .Append(". ")
                    .Append(player.Username)
                    .Append(" userId=")
                    .Append(player.UserId)
                    .Append(" clientId=")
                    .Append(player.ClientId)
                    .Append(" ready=")
                    .Append(player.IsReady)
                    .Append(" owner=")
                    .Append(player.Owner)
                    .AppendLine();
            }

            Log(builder.ToString());
        }

        private void EnsureInitialized(object service, string serviceName)
        {
            if (service is not IInitializable initializable || initializable.IsInitialized)
            {
                return;
            }

            initializable.Initialize();
            initializable.IsInitialized = true;
            Log($"{serviceName} initialized.");
        }

        private void Log(string message)
        {
            Debug.Log($"{LogPrefix} {message}");
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"{LogPrefix} {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"{LogPrefix} {message}");
        }
    }
}
