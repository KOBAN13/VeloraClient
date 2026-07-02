using Core.Utils.Screens;
using Network.Contracts;
using Network.Data;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using UI.Views;
using VContainer;

namespace UI.ViewModels
{
    public class GameListItemViewModel : ViewModel
    {
        [Inject] private ILobbyClientService _lobbyService;
        [Inject] private IScreenService _screenService;

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> PlayButtonBinder = new();

        [AutoBind] public readonly ViewModelBinder<string> PlayerInLobbyBinder = new();

        [AutoBind] public readonly ViewModelBinder<string> RoomNameBinder = new();

        private readonly ReactiveProperty<bool> _interactablePlayButton = new(true);

        public ReactiveProperty<bool> InteractablePlayButton => _interactablePlayButton;

        public RoomSummaryData RoomData { get; private set; }

        public override void Initialize()
        {
            PlayButtonBinder.Value.Subscribe(OnPlayButton).AddTo(Disposable);
        }

        public void UpdateGameListItem(RoomSummaryData room)
        {
            var playerSlots = room.MaxPlayers - room.Players.Length;

            _interactablePlayButton.Value = playerSlots > 0;

            RoomNameBinder.Value =  room.RoomName;
            PlayerInLobbyBinder.Value = $"Player slots: {playerSlots}";

            RoomData = room;
        }

        private void OnPlayButton(Unit unit)
        {
            _lobbyService.JoinRoom(RoomData.RoomId);
            _screenService.OpenSync<LobbyScreen>();
        }
    }
}