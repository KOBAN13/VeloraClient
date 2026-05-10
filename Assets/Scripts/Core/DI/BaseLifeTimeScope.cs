using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.DI
{
    public class BaseLifeTimeScope : LifetimeScope
    {
        private static readonly Dictionary<Type, BaseLifeTimeScope> BuiltScopes = new();

        protected IContainerBuilder Builder;

        protected override void Awake()
        {
            base.Awake();

            if (Container != null)
            {
                BuiltScopes[GetType()] = this;
            }
        }

        protected override void OnDestroy()
        {
            if (BuiltScopes.TryGetValue(GetType(), out var scope) && scope == this)
            {
                BuiltScopes.Remove(GetType());
            }

            base.OnDestroy();
        }

        protected override LifetimeScope FindParent()
        {
            var parentType = parentReference.Type;

            if (parentType == null)
            {
                return base.FindParent();
            }

            foreach (var scope in BuiltScopes)
            {
                if (parentType.IsAssignableFrom(scope.Key) &&
                    scope.Value != null &&
                    scope.Value.Container != null)
                {
                    return scope.Value;
                }
            }

            return base.FindParent();
        }

        protected void RegisterComponent<T>(T component) where T : Component
        {
            Builder.RegisterComponent(component).AsImplementedInterfaces().AsSelf();
        }

        protected void RegisterEntryPoint<T>(Lifetime lifetime = Lifetime.Singleton) where T : class
        {
            Builder.Register<T>(lifetime).AsImplementedInterfaces().AsSelf();
            Builder.RegisterBuildCallback(container =>
            {
                var entryPoint = container.Resolve<T>();

                if (entryPoint is not global::Core.Utils.Services.IInitializable initializable || initializable.IsInitialized)
                {
                    return;
                }

                initializable.Initialize();
                initializable.IsInitialized = true;
            });
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
