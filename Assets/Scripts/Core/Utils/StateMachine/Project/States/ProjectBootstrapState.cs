using Core.Utils.SceneManagement;
using Core.Utils.StateMachine.Abstract.States;
using Cysharp.Threading.Tasks;
using Services.SceneManagement.Enums;
using UI.Views;
using UnityEngine;

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
            LoadMainMenu().Forget(Debug.LogException);
        }

        public void Exit()
        {
        }

        private async UniTask LoadMainMenu()
        {
            await _sceneLoader.LoadScene(
                TypeScene.MainMenu,
                typeof(MainMenuScreen),
                typeof(LoginScreen),
                typeof(RegisterScreen));

            _projectStateMachine.Enter<ProjectMainMenu>();
        }
    }
}
