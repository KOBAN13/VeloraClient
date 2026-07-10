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
        [Inject] private IReadyToggleCooldownService _readyToggleCooldownService;
        
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> KickPlayerButtonBinder = new();
        [AutoBind] public readonly ViewModelBinder<EUIObjectState> KickPlayerObject = new();
        [AutoBind] public readonly ViewModelBinder<string> UserNameText = new();
        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand<bool>> ReadyPlayerToggle = new();
        [AutoBind] public readonly ViewModelBinder<float> ReadyCooldownProgress = new();
        
        public Observable<bool> InteractableToggle => _readyToggleCooldownService.ReadyToggleInteractable;

        private ulong _userId;
        
        public override void Initialize()
        {
            KickPlayerButtonBinder.Value.Subscribe(OnKickPlayerInLobby).AddTo(Disposable);
            ReadyPlayerToggle.Value.Subscribe(OnReadyPlayerToggle).AddTo(Disposable);
            
            _readyToggleCooldownService.ReadyCooldownProgress
                .Subscribe(value => ReadyCooldownProgress.Value = value)
                .AddTo(Disposable);
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

        private void OnReadyPlayerToggle(bool toggle)
        {
            _readyToggleCooldownService.TrySetReady(toggle);
        }
    }
}
