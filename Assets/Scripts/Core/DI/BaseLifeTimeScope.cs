using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.DI
{
    public class BaseLifeTimeScope : LifetimeScope
    {
        protected IContainerBuilder Builder;

        protected void RegisterComponent<T>(T component) where T : Component
        {
            Builder.RegisterComponent(component).AsImplementedInterfaces().AsSelf();
        }

        protected void RegisterEntryPoint<T>(Lifetime lifetime = Lifetime.Singleton) where T : class
        {
            Builder.RegisterEntryPoint<T>(lifetime).AsSelf();
        }

        protected void RegisterWithArgument<T, TParam>(Lifetime lifetime, TParam param) where T : class
        {
            Builder.Register<T>(lifetime).AsImplementedInterfaces().WithParameter(param).AsSelf();
        }
        
        protected void Register<T>(Lifetime lifetime) where T : class
        {
            Builder.Register<T>(lifetime).AsImplementedInterfaces().AsSelf();
        }
        
        protected void RegisterInstance<T>(T instance) where T : class
        {
            var implementationType = instance.GetType();
            Builder.RegisterInstance(instance, implementationType).AsImplementedInterfaces();
        }
    }
}
