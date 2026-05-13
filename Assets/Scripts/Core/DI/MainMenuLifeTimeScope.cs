using Network;
using VContainer;

namespace Core.DI
{
    public class MainMenuLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;

            RegisterMainMenuServices();
        }

        private void RegisterMainMenuServices()
        {
            RegisterEntryPoint<LoginClientService>();
            RegisterEntryPoint<RegisterClientService>();
        }
    }
}
