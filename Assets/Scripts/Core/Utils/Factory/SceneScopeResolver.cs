using Core.DI;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Core.Utils.Factory
{
    public class SceneScopeResolver
    {
        private readonly LifetimeScope _rootScope;

        public SceneScopeResolver(LifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        public IObjectResolver Resolve()
        {
            var activeScene = SceneManager.GetActiveScene();
            
            var sceneScope = LifetimeScope.Find<BaseLifeTimeScope>(activeScene);

            if (sceneScope != null && sceneScope.Container != null)
            {
                return sceneScope.Container;
            }

            return _rootScope.Container;
        }
    }
}
