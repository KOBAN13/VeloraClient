using System;
using System.Threading;
using Core.Utils.Screens;
using Cysharp.Threading.Tasks;
using Network.Contracts;
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
        private static readonly TimeSpan ResponseTimeout = TimeSpan.FromSeconds(3);
        private const string ConnectionErrorMessage = "Нет стабильного подключения";
        
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
        private CancellationTokenSource _responseTimeoutCancellation;
        
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
            CompleteRegisterRequest();

            if (string.IsNullOrEmpty(serverState))
                return;
            
            ShowServerError(serverState);
        }

        private void OnRegisterSucceeded(Unit unit)
        {
            CompleteRegisterRequest();
            ServerStateBannerBinder.Value = EUIObjectState.Hide;
            _screenService.CloseScreen<RegisterScreen>();
        }
        
        private void OnCloseScreen(Unit unit)
        {
            CancelResponseTimeout();
            _screenService.CloseScreen<RegisterScreen>();
            
            ServerStateTextBinder.Value = string.Empty;
            ServerStateBannerBinder.Value = EUIObjectState.Hide;
        }
        
        private void OnLoginChanged(string login) => _login = login;
        private void OnPasswordChanged(string password) => _password = password;

        private void OnRegisterRequest(Unit unit)
        {
            if (!_interactableRegisterButton.Value)
                return;

            _interactableRegisterButton.Value = false;
            ServerStateBannerBinder.Value = EUIObjectState.Hide;
            StartResponseTimeout();
            
            _registrationService.Register(_login, _password);
        }

        private void StartResponseTimeout()
        {
            CancelResponseTimeout();
            _responseTimeoutCancellation = new CancellationTokenSource();
            WaitForResponseTimeout(_responseTimeoutCancellation.Token).Forget();
        }

        private async UniTaskVoid WaitForResponseTimeout(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(ResponseTimeout, cancellationToken: token);
                
                _interactableRegisterButton.Value = true;
                ShowServerError(ConnectionErrorMessage);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CompleteRegisterRequest()
        {
            _interactableRegisterButton.Value = true;
            CancelResponseTimeout();
        }

        private void CancelResponseTimeout()
        {
            _responseTimeoutCancellation?.Cancel();
            _responseTimeoutCancellation?.Dispose();
            _responseTimeoutCancellation = null;
        }

        private void ShowServerError(string serverState)
        {
            ServerStateTextBinder.Value = serverState;
            ServerStateBannerBinder.Value = EUIObjectState.Show;
        }
        
        public override void Dispose()
        {
            CancelResponseTimeout();
            base.Dispose();
        }
    }
}
