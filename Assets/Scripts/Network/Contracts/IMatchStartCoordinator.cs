using Packets;
using R3;

namespace Network.Contracts
{
    public interface IMatchStartCoordinator
    {
        MatchStartMessage PendingMatch { get; }
        Observable<MatchStartMessage> MatchStarted { get; }
    }
}