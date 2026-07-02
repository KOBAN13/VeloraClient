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
        [Inject] private ISessionManager _sessionManager;
        
        public readonly RefTypeViewModelBinder<ReactiveCommand> KickPlayerCommand = new();
        public readonly ViewModelBinder<EUIObjectState> KickPlayerObject = new();
        public readonly ViewModelBinder<string> PingPlayerText = new();
        public readonly ViewModelBinder<string> UserNameText = new();
        
        public override void Initialize()
        {
            KickPlayerCommand.Value.Subscribe(OnKickPlayerInLobby).AddTo(Disposable);
        }
        
        public void UpdateGameListItem(string userName, string ping)
        {
            UserNameText.Value = userName;
            PingPlayerText.Value = ping;
        }
        
        public void ActivityKickPlayerButton(EUIObjectState state)
        {
            KickPlayerObject.Value = state;
        }
        
        private void OnKickPlayerInLobby(Unit unit)
        {
            var userId = _sessionManager.FindUserIdByName(UserNameText.Value);
            
            _lobbyService.KickUser(userId);
        }
    }
}