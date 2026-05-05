using Core.Utils.Screens;
using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.States;
using R3;
using UI.Core;
using UI.Helpers;
using UI.Views;
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
            _screenService.OpenSync<RegisterScreen>();
        }

        private void SignIn(Unit unit)
        {
            _screenService.OpenSync<LoginScreen>();
        }

        private void Play(Unit unit)
        {
            _projectStateMachine.Enter<ProjectGameState>();
        }
    }
}
