using Core.Utils.StateMachine.Abstract.States;

namespace Core.Utils.StateMachine.Abstract.Factory
{
    public interface IStateFactory
    {
        T GetState<T>() where T : class, IExitableState;
    }
}