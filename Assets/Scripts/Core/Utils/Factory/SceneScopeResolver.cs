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
            var sceneScope = FindActiveSceneScope();

            if (sceneScope == null)
            {
                return _rootScope.Container;
            }

            return Resolve(sceneScope);
        }

        public IObjectResolver ResolveRequiredActiveSceneScope()
        {
            return ResolveRequired(SceneManager.GetActiveScene());
        }

        public IObjectResolver ResolveRequired(Scene scene)
        {
            var sceneScope = FindSceneScope(scene);

            if (sceneScope == null)
            {
                throw new VContainerException(
                    typeof(BaseLifeTimeScope),
                    $"Scene '{scene.name}' does not contain a {nameof(BaseLifeTimeScope)}.");
            }

            return Resolve(sceneScope);
        }

        private IObjectResolver Resolve(BaseLifeTimeScope sceneScope)
        {
            if (sceneScope.Container == null)
            {
                sceneScope.Build();
            }

            if (sceneScope.Container == null)
            {
                throw new VContainerException(
                    sceneScope.GetType(),
                    $"Lifetime scope '{sceneScope.name}' was not built.");
            }

            return sceneScope.Container;
        }

        private static BaseLifeTimeScope FindActiveSceneScope()
        {
            return FindSceneScope(SceneManager.GetActiveScene());
        }

        private static BaseLifeTimeScope FindSceneScope(Scene scene)
        {
            return LifetimeScope.Find<BaseLifeTimeScope>(scene) as BaseLifeTimeScope;
        }
    }
}
