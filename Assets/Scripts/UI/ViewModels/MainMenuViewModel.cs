using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using R3;
using Services.SceneManagement.Enums;
using UI.Core;
using UI.Helpers;
using UI.Utils;
using VContainer;

namespace UI.ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private IScreenService _screenService;

        [Inject] private SceneLoader _sceneLoader;

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> SignInButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> SignUpButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> PlayButtonViewBinder = new();

        [AutoBind] public readonly RefTypeViewModelBinder<ReactiveCommand> AutoLoginButtonViewBinder = new();

        [AutoBind] public readonly ViewModelBinder<string> UserNameBinder = new();

        [AutoBind] public readonly ViewModelBinder<EUIObjectState> ObjectLoginPanel = new();

        [AutoBind] public readonly ViewModelBinder<EUIObjectState> ObjectUsernamePanel = new();

        public override void Initialize()
        {
            SignUpButtonViewBinder.Value.Subscribe(SignUp).AddTo(Disposable);
            SignInButtonViewBinder.Value.Subscribe(SignIn).AddTo(Disposable);
            PlayButtonViewBinder.Value.Subscribe(Play).AddTo(Disposable);
        }

        private void SignUp(Unit unit) {}
        private async void SignIn(Unit unit) {}

        private async void Play(Unit unit)
        {
            await _sceneLoader.LoadScene(TypeScene.Lobby);
        }
    }
}