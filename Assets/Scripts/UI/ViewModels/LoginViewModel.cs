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
        public readonly ViewModelBinder<string> ErrorBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ObjectLoginPanel = new();

        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public Observable<bool> InteractableSignInButton => _interactableSignInButton;

        private string _login;
        private string _password;
        
        public override void Initialize()
        {
            UsernameBinder.Value.Subscribe(OnLoginChanged).AddTo(Disposable);
            
            LoginBinder.Value.Subscribe(OnLoginRequest).AddTo(Disposable);
            
            PasswordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            
            CloseBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            _loginClientService.LoginErrorRequest.Subscribe(OnLoginToLobbyError).AddTo(Disposable);
            
            _loginClientService.SuccessLogin.Subscribe(OnSuccessLoginToRoom).AddTo(Disposable);
        }
        
        private void OnLoginChanged(string login) => _login = login;
        
        private void OnPasswordChanged(string password) => _password = password;

        private void OnCloseScreen(Unit unit)
        {
            _screenService.CloseScreen<LoginScreen>();
        }

        private void OnLoginToLobbyError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            
            ErrorBinder.Value = error;
            ObjectLoginPanel.Value = EUIObjectState.Show;
            _interactableSignInButton.Value = true;
        }

        private void OnSuccessLoginToRoom(Unit unit)
        {
            ObjectLoginPanel.Value = EUIObjectState.Hide;
            _interactableSignInButton.Value = true;
            _screenService.CloseScreen<LoginScreen>();
        }

        private void OnLoginRequest(Unit unit)
        { 
            if (string.IsNullOrEmpty(_login) || string.IsNullOrEmpty(_password)) 
                return;

            _interactableSignInButton.Value = false;
            
            _loginClientService.Login(_login, _password);
        }
    }
}