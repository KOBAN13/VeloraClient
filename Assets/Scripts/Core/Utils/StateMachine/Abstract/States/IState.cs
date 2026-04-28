namespace Core.Utils.StateMachine.Abstract.States
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}