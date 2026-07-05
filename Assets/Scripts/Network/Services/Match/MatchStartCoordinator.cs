using System;
using Core.Utils.Services;
using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.States;
using Network.Contracts;
using Network.Messaging;
using Packets;
using R3;

namespace Network.Services.Match
{
    public class MatchStartCoordinator : IMatchStartCoordinator, IInitializable, IDisposable
    {
        private readonly INetworkMessageBus _messages;
        private readonly IProjectStateMachine _projectStateMachine;
        private readonly CompositeDisposable _disposables = new();
        private readonly Subject<MatchStartMessage> _matchStartSubject = new();

        public MatchStartMessage PendingMatch { get; private set; }
        public Observable<MatchStartMessage> MatchStarted => _matchStartSubject;

        public bool IsInitialized { get; set; }

        public MatchStartCoordinator(
            INetworkMessageBus messages,
            IProjectStateMachine projectStateMachine)
        {
            _messages = messages;
            _projectStateMachine = projectStateMachine;
        }

        public void Initialize()
        {
            _messages.On<MatchStartMessage>()
                .Subscribe(message => OnMatchStarted(message.Payload))
                .AddTo(_disposables);
        }

        private void OnMatchStarted(MatchStartMessage matchStartMessage)
        {
            PendingMatch = matchStartMessage;
            _matchStartSubject.OnNext(matchStartMessage);
            _projectStateMachine.Enter<ProjectGameState>();
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _matchStartSubject.Dispose();
        }
    }
}