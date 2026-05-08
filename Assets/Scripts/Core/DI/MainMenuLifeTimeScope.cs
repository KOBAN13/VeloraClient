using Network;
using VContainer;

namespace Core.DI
{
    public class MainMenuLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;

            RegisterMainMenuServices(builder);
        }

        private static void RegisterMainMenuServices(IContainerBuilder builder)
        {
            builder.Register<LoginClientService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<RegisterClientService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}
