using Core.Utils.Screens;
using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.States;
using Cysharp.Threading.Tasks;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Views;
using UnityEngine;
using VContainer;

namespace UI.ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private IProjectStateMachine _projectStateMachine;
        [Inject] private IScreenService _screenService;

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> SignInButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> SignUpButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> PlayButtonViewBinder = new();

        public override void Initialize()
        {
            SignUpButtonViewBinder.Value.Subscribe(SignUp).AddTo(Disposable);
            SignInButtonViewBinder.Value.Subscribe(SignIn).AddTo(Disposable);
            PlayButtonViewBinder.Value.Subscribe(Play).AddTo(Disposable);
        }

        private void SignUp(Unit unit)
        {
            OpenRegisterScreen().Forget(Debug.LogException);
        }

        private void SignIn(Unit unit)
        {
            OpenLoginScreen().Forget(Debug.LogException);
        }

        private void Play(Unit unit)
        {
            _projectStateMachine.Enter<ProjectGameState>();
        }

        private async UniTask OpenRegisterScreen()
        {
            await _screenService.OpenAsync<RegisterScreen>();
        }

        private async UniTask OpenLoginScreen()
        {
            await _screenService.OpenAsync<LoginScreen>();
        }
    }
}
