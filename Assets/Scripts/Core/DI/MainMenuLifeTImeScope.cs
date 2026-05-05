using VContainer;

namespace Core.DI
{
    public class MainMenuLifeTImeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;

        }

        private void RegisterServices()
        {
            
        }
    }
}