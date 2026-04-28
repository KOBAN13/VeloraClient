using Core.Utils.StateMachine.Abstract.Factory;
using VContainer;

namespace Core.Utils.StateMachine.Project.Factory
{
    public sealed class ProjectAStateFactory : AStateFactory, IProjectStateFactory
    {
        public ProjectAStateFactory(IObjectResolver container) : base(container)
        {
        }
    }
}
