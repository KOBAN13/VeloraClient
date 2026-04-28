using Core.Utils.StateMachine.Abstract.States;

namespace Core.Utils.StateMachine.Abstract
{
    public interface IStateMachine 
    {
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
        IExitableState ActiveState { get; }
    }
}