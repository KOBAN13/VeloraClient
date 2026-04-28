using Core.Utils.StateMachine.Abstract.States;
using VContainer;

namespace Core.Utils.StateMachine.Abstract.Factory
{
    public abstract class AStateFactory : IStateFactory
    {
        private readonly IObjectResolver _container;

        protected AStateFactory(IObjectResolver container)
        {
            _container = container;
        }

        public T GetState<T>() where T : class, IExitableState
        {
            return _container.Resolve<T>();
        }
    }
}