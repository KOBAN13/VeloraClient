using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Core.Utils.Pool;
using Core.Utils.Screens;
using Network.Contracts;
using Network.Data;
using ObservableCollections;
using R3;
using UI.Core;
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

        public readonly RefTypeViewModelBinder<ReactiveCommand> InvitePlayerCommand = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> StartGameCommand = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> LeaveGameCommand = new();
        public readonly ViewModelBinder<EUIObjectState> ObjectStartGameCommand = new();

        public readonly ReactiveCommand<GameObject> SetParentObject = new();

        private bool _isLobbyInitialized;

        public override void Initialize()
        {
            SetParentObject.Subscribe(parent => InitializeLobbyAsync(parent).Forget()).AddTo(Disposable);

            InvitePlayerCommand.Value.Subscribe(OnInvitePlayer).AddTo(Disposable);
            StartGameCommand.Value.Subscribe(OnStartGame).AddTo(Disposable);
            LeaveGameCommand.Value.Subscribe(OnLeaveGame).AddTo(Disposable);

            _lobbyService.KickedUser.Subscribe(_ => OnKickedFromLobby()).AddTo(Disposable);
            _roomStateService.CurrentRoomChanged.Subscribe(_ => UpdateStartGameButtonVisibility()).AddTo(Disposable);

            UpdateStartGameButtonVisibility();
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
            ObjectStartGameCommand.Value = _roomStateService.IsOwner
                ? EUIObjectState.Show
                : EUIObjectState.Hide;
        }

        private void OnLeaveGame(Unit unit)
        {
            _playerLobbyItemPool.Clear();
            _lobbyService.LeaveRoom();
            CloseLobbyScreen();
        }

        private async UniTask InitializeLobbyAsync(GameObject parent)
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
            _playerLobbyItemPool.Clear();
            CloseLobbyScreen();
        }

        private void CloseLobbyScreen()
        {
            _screenService.CloseScreen<LobbyScreen>();
            _screenService.OpenSync<GameRoomHubScreen>();
        }
    }
}