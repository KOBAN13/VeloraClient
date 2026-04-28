using Core.Utils.Services;
using Core.Utils.StateMachine.Abstract.Factory;
using Core.Utils.StateMachine.Abstract.States;

namespace Core.Utils.StateMachine.Abstract
{
    public abstract class AStateMachine : IStateMachine, ITickable
    {
        private readonly IStateFactory _stateFactory;
        public IExitableState ActiveState { get; private set; }

        protected AStateMachine(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
        
        
        public void Tick(float deltaTime)
        {
            if (ActiveState is IUpdateable updateableState)
            {
                updateableState.Update();
            }
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            ActiveState?.Exit();

            var state = _stateFactory.GetState<TState>();
            ActiveState = state;

            return state;
        }
    }
}
