using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.States;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Core.DI.Bootstraps
{
    public class RootBootstrap : Bootstrap<RootLifeTimeScope>
    {
        [Inject] private IProjectStateMachine _projectStateMachine;

        protected override UniTask Initialize()
        {
            InitializeServices();

            DontDestroyOnLoad(gameObject);

            _projectStateMachine.Enter<ProjectBootstrapState>();

            return UniTask.CompletedTask;
        }
    }
}
