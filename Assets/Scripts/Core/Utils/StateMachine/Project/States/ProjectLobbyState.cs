using Core.Utils.Screens;
using Core.Utils.StateMachine.Abstract.States;
using Cysharp.Threading.Tasks;
using UI.Views;

namespace Core.Utils.StateMachine.Project.States
{
    public class ProjectLobbyState : IState
    {
        private readonly ScreenService _screenService;

        public ProjectLobbyState(ScreenService screenService)
        {
            _screenService = screenService;
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
            await _screenService.OpenAsync<GameRoomHubScreen>();
            
            var lobbyScreen = await _screenService.OpenAsync<LobbyScreen>();
            lobbyScreen.gameObject.SetActive(false);
            
            var createRoomScreen = await _screenService.OpenAsync<CreateRoomScreen>();
            createRoomScreen.gameObject.SetActive(false);
        }
    }
}