namespace Core.Utils.StateMachine.Abstract.States
{
    public interface IPayloadState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
}