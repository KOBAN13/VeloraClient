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

            RegisterGameServices(builder);
        }

        private static void RegisterGameServices(IContainerBuilder builder)
        {
            builder.Register<PoolService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ChatService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ChatClientService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GameAStateFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GameAStateMachine>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GameBootstrapState>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GameMainState>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}
