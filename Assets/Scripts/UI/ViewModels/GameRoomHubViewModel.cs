using Cysharp.Threading.Tasks;
using Core.Utils.Pool;
using Core.Utils.Screens;
using Network.Contracts;
using Network.Data;
using ObservableCollections;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Views;
using UnityEngine;
using VContainer;

namespace UI.ViewModels
{
    public class GameRoomHubViewModel : ViewModel
    {
        [Inject] private IRoomStateService _roomStateService;
        [Inject] private ILoginClientService _loginClientService;
        [Inject] private IScreenService _screenService;
        [Inject] private IGameListItemPool _gameListItemPool;

        [AutoBind] public readonly ViewModelBinder<string> UserNameBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> CreateRoomButtonBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> LogoutButtonBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> AutoLobbyButtonBinder = new();
        [AutoBind] public readonly ReactiveCommand<GameObject> SetParentObject = new();

        public override void Initialize()
        {
            SetParentObject.Subscribe(parent => InitUIAsync(parent).Forget()).AddTo(Disposable);
            
            UserNameBinder.Value = _loginClientService.UserName;

            CreateRoomButtonBinder.Value.Subscribe(_ => OnCreateRoom()).AddTo(Disposable);
            LogoutButtonBinder.Value.Subscribe(_ => OnLogout()).AddTo(Disposable);

            _roomStateService.RoomSummaryData.ObserveAdd().Subscribe(kvp => OnRoomAdded(kvp.Value)).AddTo(Disposable);
            _roomStateService.RoomSummaryData.ObserveRemove().Subscribe(kvp => OnRoomRemoved(kvp.Value)).AddTo(Disposable);
            _roomStateService.RoomSummaryData.ObserveReplace().Subscribe(kvp => OnRoomUpdated(kvp.NewValue)).AddTo(Disposable);

        }

        private async UniTaskVoid InitUIAsync(GameObject parent)
        {
            await _gameListItemPool.Initialize(parent);
            
            foreach (var t in _roomStateService.RoomSummaryData)
            {
                UpsertRoomItem(t);
            }

            _roomStateService.RefreshRooms();
        }

        private void OnCreateRoom() => _screenService.OpenSync<CreateRoomScreen>();

        private void OnLogout()
        {

        }

        private void OnRoomAdded(RoomSummaryData roomSummaryData)
        {
            UpsertRoomItem(roomSummaryData);
        }

        private void OnRoomRemoved(RoomSummaryData roomSummaryData)
        {
            _gameListItemPool.ReleaseListItem(roomSummaryData.RoomId);
        }

        private void OnRoomUpdated(RoomSummaryData roomSummaryData)
        {
            UpsertRoomItem(roomSummaryData);
        }

        private void UpsertRoomItem(RoomSummaryData roomSummaryData)
        {
            var item = _gameListItemPool.GetById(roomSummaryData.RoomId) ?? _gameListItemPool.GetListItem(roomSummaryData.RoomId);

            item.ViewModel.UpdateGameListItem(roomSummaryData);
        }
    }
}
