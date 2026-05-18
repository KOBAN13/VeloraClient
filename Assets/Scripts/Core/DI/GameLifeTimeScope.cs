using Core.Utils.Pool;
using Core.Utils.StateMachine.Game;
using Core.Utils.StateMachine.Game.Factory;
using Core.Utils.StateMachine.Game.States;
using Network;
using UI.Services;
using VContainer;

namespace Core.DI
{
    public class GameLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;

            RegisterGameServices();
            RegisterLobbyServices();
        }

        private void RegisterGameServices()
        {
            Register<PoolService>(Lifetime.Singleton);
            Register<ChatService>(Lifetime.Singleton);
            RegisterEntryPoint<ChatClientService>();
            Register<GameAStateFactory>(Lifetime.Singleton);
            Register<GameAStateMachine>(Lifetime.Singleton);
            Register<GameBootstrapState>(Lifetime.Singleton);
            Register<GameMainState>(Lifetime.Singleton);
        }

        private void RegisterLobbyServices()
        {
            Register<LobbyClientService>(Lifetime.Singleton);
            Register<ClientIdentityService>(Lifetime.Singleton);
            Register<RoomStateService>(Lifetime.Singleton);
        }
    }
}
