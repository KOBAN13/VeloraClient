using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.States;
using R3;
using UI.Core;
using UI.Helpers;
using VContainer;

namespace UI.ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private IProjectStateMachine _projectStateMachine;

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> SignInButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> SignUpButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> PlayButtonViewBinder = new();

        public override void Initialize()
        {
            SignUpButtonViewBinder.Value.Subscribe(SignUp).AddTo(Disposable);
            SignInButtonViewBinder.Value.Subscribe(SignIn).AddTo(Disposable);
            PlayButtonViewBinder.Value.Subscribe(Play).AddTo(Disposable);
        }

        private void SignUp(Unit unit) {}
        private void SignIn(Unit unit) {}

        private void Play(Unit unit)
        {
            _projectStateMachine.Enter<ProjectGameState>();
        }
    }
}
