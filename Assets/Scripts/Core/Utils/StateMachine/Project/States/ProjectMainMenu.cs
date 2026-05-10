using Core.Utils.Screens;
using Core.Utils.StateMachine.Abstract.States;
using Cysharp.Threading.Tasks;
using UI.Views;
using UnityEngine;

namespace Core.Utils.StateMachine.Project.States
{
    public class ProjectMainMenu : IState
    {
        private readonly IScreenService _screenService;

        public ProjectMainMenu(IScreenService screenService)
        {
            _screenService = screenService;
        }

        public void Exit()
        {
            _screenService.CloseScreen<MainMenuScreen>();
        }

        public void Enter()
        {
            OpenMainMenu().Forget(Debug.LogException);
        }

        private async UniTask OpenMainMenu()
        {
            await _screenService.OpenAsync<MainMenuScreen>();
        }
    }
}
