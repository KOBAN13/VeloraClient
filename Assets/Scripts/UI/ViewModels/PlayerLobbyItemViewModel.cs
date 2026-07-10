using Network.Contracts;
using R3;
using UI.Binders.Data;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using UI.Views.Config;
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
        [AutoBind] public readonly ViewModelBinder<EUIObjectState> ReadyCooldownObject = new();
        [AutoBind] public readonly ViewModelBinder<ImageViewData> ReadyStateImage = new();

        public Observable<bool> ReadyToggleInteractable => _readyToggleInteractable;
        public Observable<bool> ReadyToggleState => _readyToggleState;

        [Inject] private IPlayerItemLobbyParameters _parameters;
        private readonly ReactiveProperty<bool> _readyToggleInteractable = new();
        private readonly ReactiveProperty<bool> _readyToggleState = new();
        private ulong _userId;
        private bool _isReady;
        private bool _isReadyToggleAllowed;
        private bool _isReadyCooldownAvailable = true;

        public override void Initialize()
        {
            KickPlayerButtonBinder.Value.Subscribe(OnKickPlayerInLobby).AddTo(Disposable);
            ReadyPlayerToggle.Value.Subscribe(OnReadyPlayerToggle).AddTo(Disposable);

            _readyToggleCooldownService.ReadyToggleInteractable
                .Subscribe(value =>
                {
                    _isReadyCooldownAvailable = value;
                    UpdateReadyToggleInteractable();
                })
                .AddTo(Disposable);

            _readyToggleCooldownService.ReadyCooldownProgress
                .Subscribe(value => ReadyCooldownProgress.Value = value)
                .AddTo(Disposable);
        }

        public void UpdatePlayer(ulong userId, string userName, string ping, bool isReady, bool canToggleReady)
        {
            _userId = userId;
            _isReady = isReady;
            _isReadyToggleAllowed = canToggleReady;
            UserNameText.Value = userName;
            UpdateReadyState(isReady);
            UpdateReadyCooldownVisibility();
            UpdateReadyToggleInteractable();
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
            if (!_isReadyToggleAllowed)
            {
                UpdateReadyState(_isReady);
                return;
            }

            _readyToggleCooldownService.TrySetReady(toggle);
            UpdateReadyState(_isReady);
        }

        private void UpdateReadyState(bool isReady)
        {
            _readyToggleState.Value = isReady;

            ReadyStateImage.Value = isReady
                ? new ImageViewData(_parameters.PlayerReadySprite, _parameters.PlayerReadyColor)
                : new ImageViewData(_parameters.PlayerNotReadySprite, _parameters.PlayerNotReadyColor);
        }

        private void UpdateReadyToggleInteractable()
        {
            _readyToggleInteractable.Value = _isReadyToggleAllowed && _isReadyCooldownAvailable;
        }

        private void UpdateReadyCooldownVisibility()
        {
            ReadyCooldownObject.Value = _isReadyToggleAllowed
                ? EUIObjectState.Show
                : EUIObjectState.Hide;
        }

        public override void Dispose()
        {
            _readyToggleInteractable.Dispose();
            _readyToggleState.Dispose();

            base.Dispose();
        }
    }
}
