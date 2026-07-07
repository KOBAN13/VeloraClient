using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Core.Utils.StateMachine.Abstract.States;
using Cysharp.Threading.Tasks;
using Services.SceneManagement.Enums;
using UI.Views;

namespace Core.Utils.StateMachine.Project.States
{
    public class ProjectLobbyState : IState
    {
        private readonly ScreenService _screenService;
        private readonly SceneLoader _sceneLoader;

        public ProjectLobbyState(ScreenService screenService, SceneLoader sceneLoader)
        {
            _screenService = screenService;
            _sceneLoader = sceneLoader;
        }

        public void Exit()
        {
            _screenService.CloseScreen<LobbyScreen>();
            _screenService.CloseScreen<CreateRoomScreen>();
            _screenService.CloseScreen<GameRoomHubScreen>();
        }

        public void Enter()
        {
            OpenLobby().Forget();
        }

        private async UniTaskVoid OpenLobby()
        {
            await _sceneLoader.LoadScene(TypeScene.Lobby, typeof(LobbyScreen), typeof(CreateRoomScreen));

            await _screenService.OpenAsync<GameRoomHubScreen>();
        }
    }
}
