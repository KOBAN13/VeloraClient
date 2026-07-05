using System;
using Core.Utils.Services;
using Network.Contracts;
using Packets;

namespace Network.Services.Match
{
    public class MatchService : IMatchService, IInitializable
    {
        private readonly IMatchStartCoordinator _matchStartCoordinator;

        public MatchStartMessage CurrentMatch { get; private set; }
        public bool IsInitialized { get; set; }

        public MatchService(IMatchStartCoordinator matchStartCoordinator)
        {
            _matchStartCoordinator = matchStartCoordinator;
        }

        public void Initialize()
        {
            CurrentMatch = _matchStartCoordinator.PendingMatch ?? throw new InvalidOperationException(
                $"{nameof(MatchService)} cannot start without a pending match.");
        }
    }
}