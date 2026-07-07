using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Core.Utils.Pool;
using Core.Utils.Screens;
using Network.Contracts;
using Network.Data;
using ObservableCollections;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using UI.Views;
using UnityEngine;
using VContainer;

namespace UI.ViewModels
{
    public class LobbyViewModel : ViewModel
    {
        [Inject] private ILobbyClientService _lobbyService;
        [Inject] private IRoomStateService _roomStateService;
        [Inject] private IScreenService _screenService;
        [Inject] private IPlayerLobbyItemPool _playerLobbyItemPool;

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> InvitePlayerButtonBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> StartGameButtonBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> LeaveGameButtonBinder = new();
        [AutoBind] public readonly ViewModelBinder<EUIObjectState> ObjectStartGameButtonBinder = new();

        public readonly ReactiveCommand<GameObject> SetParentObject = new();

        private bool _isLobbyInitialized;
        private bool _hasRequestedPlayers;
        private ulong _requestedPlayersRoomId;

        public override void Initialize()
        {
            SetParentObject.Subscribe(parent => InitializeLobbyAsync(parent).Forget()).AddTo(Disposable);

            InvitePlayerButtonBinder.Value.Subscribe(OnInvitePlayer).AddTo(Disposable);
            StartGameButtonBinder.Value.Subscribe(OnStartGame).AddTo(Disposable);
            LeaveGameButtonBinder.Value.Subscribe(OnLeaveGame).AddTo(Disposable);

            _lobbyService.KickedUser.Subscribe(_ => OnKickedFromLobby()).AddTo(Disposable);
            
            _roomStateService.CurrentRoomChanged.Subscribe(OnCurrentRoomChanged).AddTo(Disposable);

            UpdateStartGameButtonVisibility();
        }

        private void OnCurrentRoomChanged(RoomStateData room)
        {
            RequestPlayersInCurrentRoom(room);
            UpdateStartGameButtonVisibility();
        }

        private void RequestPlayersInCurrentRoom(RoomStateData room)
        {
            if (room == null)
            {
                return;
            }

            if (_hasRequestedPlayers && _requestedPlayersRoomId == room.RoomId)
            {
                return;
            }

            _hasRequestedPlayers = true;
            _requestedPlayersRoomId = room.RoomId;

            _lobbyService.GetPlayersInLobby(room.RoomId);
        }

        private void OnInvitePlayer(Unit unit)
        {

        }

        private void OnStartGame(Unit unit)
        {
            _lobbyService.StartGame();
        }

        private void UpdateStartGameButtonVisibility()
        {
            ObjectStartGameButtonBinder.Value = _roomStateService.IsOwner
                ? EUIObjectState.Show
                : EUIObjectState.Hide;
        }

        private void OnLeaveGame(Unit unit)
        {
            ResetRequestedPlayersRoom();
            _playerLobbyItemPool.Clear();
            _lobbyService.LeaveRoom();
            CloseLobbyScreen();
        }

        private async UniTaskVoid InitializeLobbyAsync(GameObject parent)
        {
            if (_isLobbyInitialized)
            {
                return;
            }

            _isLobbyInitialized = true;

            await _playerLobbyItemPool.Initialize(parent);

            var currentPlayers = new List<PlayerData>();

            foreach (var player in _lobbyService.Players)
            {
                currentPlayers.Add(player);
            }

            _lobbyService.Players
                .ObserveAdd()
                .Subscribe(kvp => OnUserAdded(kvp.Value))
                .AddTo(Disposable);
            
            _lobbyService.Players
                .ObserveRemove()
                .Subscribe(kvp => OnUserRemoved(kvp.Value))
                .AddTo(Disposable);
            
            _lobbyService.Players
                .ObserveReplace()
                .Subscribe(kvp => OnUserUpdated(kvp.NewValue))
                .AddTo(Disposable);

            foreach (var player in currentPlayers)
            {
                OnUserAdded(player);
            }
        }

        private void OnUserAdded(PlayerData playerData)
        {
            UpdatePlayerItem(playerData);
        }

        private void OnUserRemoved(PlayerData playerData)
        {
            _playerLobbyItemPool.ReleaseListItem(playerData.UserId);
        }

        private void OnUserUpdated(PlayerData playerData)
        {
            UpdatePlayerItem(playerData);
        }

        private void UpdatePlayerItem(PlayerData playerData)
        {
            var item = _playerLobbyItemPool.GetById(playerData.UserId) ?? _playerLobbyItemPool.GetListItem(playerData.UserId);
            var kickButtonState = _roomStateService.IsOwner && !playerData.IsOwner
                ? EUIObjectState.Show
                : EUIObjectState.Hide;

            item.ViewModel.UpdatePlayer(playerData.UserId, playerData.Username, string.Empty);
            item.ViewModel.ActivityKickPlayerButton(kickButtonState);
        }

        private void OnKickedFromLobby()
        {
            ResetRequestedPlayersRoom();
            _playerLobbyItemPool.Clear();
            CloseLobbyScreen();
        }

        private void ResetRequestedPlayersRoom()
        {
            _hasRequestedPlayers = false;
            _requestedPlayersRoomId = 0;
        }

        private void CloseLobbyScreen()
        {
            _screenService.CloseScreen<LobbyScreen>();
            _screenService.OpenSync<GameRoomHubScreen>();
        }
    }
}
