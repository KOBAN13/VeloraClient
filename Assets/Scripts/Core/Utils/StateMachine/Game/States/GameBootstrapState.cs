using Core.Utils.StateMachine.Abstract.States;

namespace Core.Utils.StateMachine.Game.States
{
    public sealed class GameBootstrapState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;

        public GameBootstrapState(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            _gameStateMachine.Enter<GameMainState>();
        }

        public void Exit()
        {
            
        }
    }
}