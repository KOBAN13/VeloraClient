using Core.Utils.Screens;
using Network;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using UI.Views;
using VContainer;

namespace UI.ViewModels
{
    public class RegisterViewModel : ViewModel
    { 
        [Inject] 
        private IRegisterClientService _registrationService;
        
        [Inject] 
        private IScreenService _screenService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> LoginBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> PasswordBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> RegisterBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> CloseBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<string> ServerStateTextBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ServerStateBannerBinder = new();
        
        private readonly ReactiveProperty<bool> _interactableRegisterButton = new(true);
        
        public ReadOnlyReactiveProperty<bool> InteractableRegisterButton => _interactableRegisterButton;
        
        private string _login = string.Empty;
        private string _password = string.Empty;
        
        public override void Initialize()
        {
            LoginBinder.Value.Subscribe(OnLoginChanged).AddTo(Disposable);
            PasswordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            RegisterBinder.Value.Subscribe(OnRegisterRequest).AddTo(Disposable);
            CloseBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            _registrationService.RegisterErrorRequest.Subscribe(OnRegisterDenied).AddTo(Disposable);
            _registrationService.SuccessRegister.Subscribe(OnRegisterSucceeded).AddTo(Disposable);
        }

        private void OnRegisterDenied(string serverState)
        {
            _interactableRegisterButton.Value = true;

            if (string.IsNullOrEmpty(serverState))
                return;
            
            ServerStateTextBinder.Value = serverState;
            ServerStateBannerBinder.Value = EUIObjectState.Show;
        }

        private void OnRegisterSucceeded(Unit unit)
        {
            ServerStateBannerBinder.Value = EUIObjectState.Hide;
            _screenService.CloseScreen<RegisterScreen>();
        }
        
        private void OnCloseScreen(Unit unit) => _screenService.CloseScreen<RegisterScreen>();
        private void OnLoginChanged(string login) => _login = login;
        private void OnPasswordChanged(string password) => _password = password;

        private void OnRegisterRequest(Unit unit)
        {
            if (!_interactableRegisterButton.Value)
                return;

            _interactableRegisterButton.Value = false;
            
            _registrationService.Register(_login, _password);
        }
    }
}
