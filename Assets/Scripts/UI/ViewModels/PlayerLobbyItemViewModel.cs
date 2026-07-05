using Network.Contracts;
using R3;
using UI.Core;
using UI.Utils;
using VContainer;

namespace UI.ViewModels
{
    public class PlayerLobbyItemViewModel : ViewModel
    {
        [Inject] private ILobbyClientService _lobbyService;
        
        public readonly RefTypeViewModelBinder<ReactiveCommand> KickPlayerCommand = new();
        public readonly ViewModelBinder<EUIObjectState> KickPlayerObject = new();
        public readonly ViewModelBinder<string> PingPlayerText = new();
        public readonly ViewModelBinder<string> UserNameText = new();

        private ulong _userId;
        
        public override void Initialize()
        {
            KickPlayerCommand.Value.Subscribe(OnKickPlayerInLobby).AddTo(Disposable);
        }
        
        public void UpdatePlayer(ulong userId, string userName, string ping)
        {
            _userId = userId;
            UserNameText.Value = userName;
            PingPlayerText.Value = ping;
        }
        
        public void ActivityKickPlayerButton(EUIObjectState state)
        {
            KickPlayerObject.Value = state;
        }
        
        private void OnKickPlayerInLobby(Unit unit)
        {
            if (_userId == 0)
            {
                return;
            }

            _lobbyService.KickUser(_userId);
        }
    }
}