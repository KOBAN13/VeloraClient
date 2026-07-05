using Core.Utils.Pool;
using Network.Data;
using Network.Services.Lobby;
using VContainer;

namespace Core.DI
{
    public class LobbyLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;

            RegisterLobbyServices();
        }

        private void RegisterLobbyServices()
        {
            Register<GameListItemPool>(Lifetime.Singleton);
            Register<PlayerLobbyItemPool>(Lifetime.Singleton);
            Register<RoomStateData>(Lifetime.Singleton);
            Register<LobbyClientService>(Lifetime.Singleton);
        }
    }
}