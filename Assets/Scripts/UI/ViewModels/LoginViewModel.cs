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
    public class LoginViewModel : ViewModel
    { 
        [Inject] private ILoginClientService _loginClientService;
        
        [Inject]
        private IScreenService _screenService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> UsernameBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> PasswordBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> LoginBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> CloseBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<string> ServerStateTextBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ServerStateBannerBinder = new();

        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public Observable<bool> InteractableSignInButton => _interactableSignInButton;

        private string _username;
        private string _password;
        
        public override void Initialize()
        {
            UsernameBinder.Value.Subscribe(OnUsernameChanged).AddTo(Disposable);
            
            LoginBinder.Value.Subscribe(OnLoginRequest).AddTo(Disposable);
            
            PasswordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            
            CloseBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            _loginClientService.LoginErrorRequest.Subscribe(OnLoginDenied).AddTo(Disposable);
            
            _loginClientService.SuccessLogin.Subscribe(OnLoginSucceeded).AddTo(Disposable);
        }
        
        private void OnUsernameChanged(string username) => _username = username;
        
        private void OnPasswordChanged(string password) => _password = password;

        private void OnCloseScreen(Unit unit)
        {
            _screenService.CloseScreen<LoginScreen>();
        }

        private void OnLoginDenied(string serverState)
        {
            _interactableSignInButton.Value = true;

            if (string.IsNullOrEmpty(serverState))
                return;
            
            ServerStateTextBinder.Value = serverState;
            ServerStateBannerBinder.Value = EUIObjectState.Show;
        }

        private void OnLoginSucceeded(Unit unit)
        {
            ServerStateBannerBinder.Value = EUIObjectState.Hide;
            _screenService.CloseScreen<LoginScreen>();
        }

        private void OnLoginRequest(Unit unit)
        {
            if (!_interactableSignInButton.Value)
                return;

            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                return;

            _interactableSignInButton.Value = false;
            
            _loginClientService.Login(_username, _password);
        }
    }
}
