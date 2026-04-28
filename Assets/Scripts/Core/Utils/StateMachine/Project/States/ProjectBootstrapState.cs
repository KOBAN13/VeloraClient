using Core.Utils.SceneManagement;
using Core.Utils.StateMachine.Abstract.States;
using Cysharp.Threading.Tasks;
using Services.SceneManagement.Enums;

namespace Core.Utils.StateMachine.Project.States
{
    public sealed class ProjectBootstrapState : IState
    {
        private readonly IProjectStateMachine _projectStateMachine;
        private readonly SceneLoader _sceneLoader;

        public ProjectBootstrapState(IProjectStateMachine projectStateMachine, SceneLoader sceneLoader)
        {
            _projectStateMachine = projectStateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            LoadMainMenu().Forget();
        }

        public void Exit()
        {
        }

        private async UniTaskVoid LoadMainMenu()
        {
            await _sceneLoader.LoadScene(TypeScene.MainMenu);
            _projectStateMachine.Enter<ProjectMainMenu>();
        }
    }
}
