using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Core.Utils.StateMachine.Abstract.States;
using Cysharp.Threading.Tasks;
using Services.SceneManagement.Enums;
using UI.Views;

namespace Core.Utils.StateMachine.Project.States
{
    public sealed class ProjectGameState : IState
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IScreenService _screenService;

        public ProjectGameState(SceneLoader sceneLoader, IScreenService screenService)
        {
            _sceneLoader = sceneLoader;
            _screenService = screenService;
        }

        public void Enter()
        {
            LoadGame().Forget();
        }

        public void Exit()
        {
        }

        private async UniTaskVoid LoadGame()
        {
            await _sceneLoader.LoadScene(TypeScene.Game, typeof(ChatScreen));
            await _screenService.OpenAsync<ChatScreen>();
        }
    }
}
