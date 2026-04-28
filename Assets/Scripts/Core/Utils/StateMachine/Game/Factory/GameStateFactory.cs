using Core.Utils.StateMachine.Abstract.Factory;
using VContainer;

namespace Core.Utils.StateMachine.Game.Factory
{
    public sealed class GameAStateFactory : AStateFactory, IGameStateFactory
    {
        public GameAStateFactory(IObjectResolver container) : base(container)
        {
        }
    }
}