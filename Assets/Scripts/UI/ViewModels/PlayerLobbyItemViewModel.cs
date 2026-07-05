using Network.Contracts;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using VContainer;

namespace UI.ViewModels
{
    public class PlayerLobbyItemViewModel : ViewModel
    {
        [Inject] private ILobbyClientService _lobbyService;
        
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> KickPlayerCommand = new();
        [AutoBind] public readonly ViewModelBinder<EUIObjectState> KickPlayerObject = new();
        [AutoBind] public readonly ViewModelBinder<string> UserNameText = new();

        private ulong _userId;
        
        public override void Initialize()
        {
            KickPlayerCommand.Value.Subscribe(OnKickPlayerInLobby).AddTo(Disposable);
        }
        
        public void UpdatePlayer(ulong userId, string userName, string ping)
        {
            _userId = userId;
            UserNameText.Value = userName;
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