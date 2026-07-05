using Packets;

namespace Network.Contracts
{
    public interface IMatchService
    {
        MatchStartMessage CurrentMatch { get; }
        void Initialize();
    }
}