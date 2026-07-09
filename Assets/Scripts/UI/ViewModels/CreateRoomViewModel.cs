using Core.Utils.Screens;
using Network.Contracts;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using UI.Views;
using VContainer;

namespace UI.ViewModels
{
    public class CreateRoomViewModel : ViewModel
    {
        [Inject] private IRoomStateService _roomStateService;
        [Inject] private IScreenService _screenService;
        
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> CreateRoomButtonBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand<string>> RoomNameTextViewBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand<string>> RoomMaxPlayersTextViewBinder = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> CloseScreenButtonBinder = new();

        private readonly ReactiveProperty<bool> _interactableCreateRoomButton = new(true);

        public Observable<bool> InteractableCreateRoomButton => _interactableCreateRoomButton;
        
        private uint _maxPlayers;
        private string _roomName;

        public override void Initialize()
        {
            CreateRoomButtonBinder.Value.Subscribe(OnCreateRoom).AddTo(Disposable);
            RoomNameTextViewBinder.Value.Subscribe(OnRoomNameChanged).AddTo(Disposable);
            RoomMaxPlayersTextViewBinder.Value.Subscribe(OnMaxPlayersChanged).AddTo(Disposable);
            CloseScreenButtonBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            _roomStateService.CurrentRoomChanged
                .Where(_ => _roomStateService.IsOwner)
                .Subscribe(_ => OnOpenLobbyScreen())
                .AddTo(Disposable);
        }

        private void OnCreateRoom(Unit unit)
        {
            _roomStateService.CreateRoom(_roomName, _maxPlayers);

            _interactableCreateRoomButton.Value = false;
        }
        
        private void OnRoomNameChanged(string roomName)
        {
            _roomName = roomName;
        }
        
        private void OnMaxPlayersChanged(string maxPlayers)
        {
            _maxPlayers = uint.Parse(maxPlayers);
        }

        private void OnCloseScreen(Unit unit)
        {
            _screenService.CloseScreen<CreateRoomScreen>();
        }

        private void OnOpenLobbyScreen()
        {
            _screenService.CloseScreen<CreateRoomScreen>();
            _screenService.CloseScreen<GameRoomHubScreen>();
            _screenService.OpenSync<LobbyScreen>();
        }
    }
}
