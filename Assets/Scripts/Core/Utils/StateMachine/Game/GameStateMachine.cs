using Core.Utils.StateMachine.Abstract;
using Core.Utils.StateMachine.Game.Factory;

namespace Core.Utils.StateMachine.Game
{
    public sealed class GameAStateMachine : AStateMachine, IGameStateMachine
    {
        public GameAStateMachine(IGameStateFactory stateFactory) : base(stateFactory)
        {
        }
    }
}
