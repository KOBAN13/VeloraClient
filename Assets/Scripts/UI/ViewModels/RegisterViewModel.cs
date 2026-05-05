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
        public readonly ViewModelBinder<string> ErrorBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ObjectLoginPanel = new();
        
        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public ReadOnlyReactiveProperty<bool> InteractableSignInButton => _interactableSignInButton;
        
        private string _email = string.Empty;
        private string _login = string.Empty;
        private string _password = string.Empty;
        
        public override void Initialize()
        {
            LoginBinder.Value.Subscribe(OnLoginChanged).AddTo(Disposable);
            PasswordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            RegisterBinder.Value.Subscribe(OnRegisterRequest).AddTo(Disposable);
            CloseBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            _registrationService.RegisterErrorRequest.Subscribe(OnError).AddTo(Disposable);
            _registrationService.SuccessRegister.Subscribe(OnRegistration).AddTo(Disposable);
        }

        private void OnError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            
            ErrorBinder.Value = error;
            ObjectLoginPanel.Value = EUIObjectState.Show;
            _interactableSignInButton.Value = true;
        }

        private void OnRegistration(Unit unit)
        {
            ObjectLoginPanel.Value = EUIObjectState.Hide;
            _interactableSignInButton.Value = true;
            _screenService.CloseScreen<RegisterScreen>();
        }
        
        private void OnCloseScreen(Unit unit) => _screenService.CloseScreen<RegisterScreen>();
        private void OnLoginChanged(string login) => _login = login;
        private void OnPasswordChanged(string password) => _password = password;

        private void OnRegisterRequest(Unit unit)
        {
            _interactableSignInButton.Value = false;
            
            _registrationService.Register(_login, _password);
        }
    }
}