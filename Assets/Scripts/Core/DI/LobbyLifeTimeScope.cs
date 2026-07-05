using VContainer;

namespace Core.DI
{
    public class LobbyLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
        }
    }
}
