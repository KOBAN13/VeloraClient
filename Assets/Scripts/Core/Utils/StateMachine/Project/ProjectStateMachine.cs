using Core.Utils.StateMachine.Abstract;
using Core.Utils.StateMachine.Project.Factory;

namespace Core.Utils.StateMachine.Project
{
    public sealed class ProjectAStateMachine : AStateMachine, IProjectStateMachine
    {
        public ProjectAStateMachine(IProjectStateFactory stateFactory) : base(stateFactory)
        {
        }
    }
}
