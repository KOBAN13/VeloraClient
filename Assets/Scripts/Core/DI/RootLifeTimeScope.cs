using System.Linq;
using Core.Utils.Factory;
using Core.Utils.Logger;
using Core.Utils.Logger.Data;
using Core.Utils.Pool;
using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Factories;
using Network;
using Services.SceneManagement;
using UI.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Core.DI
{
    public class RootLifeTimeScope : BaseLifeTimeScope
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private AssetLabelReference _configLabel;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            
            RegisterComponents();
            RegisterConfigs();
            RegisterFallbackConfigs();
            RegisterFactories();
            RegisterServices();
            RegisterEntryPoints();
        }

        private void RegisterComponents()
        {
            RegisterComponent(_sceneLoader);
        }

        private void RegisterConfigs()
        {
            var configs = Addressables
                .LoadAssetsAsync<ScriptableObject>(_configLabel, null)
                .WaitForCompletion()
                .ToList();

            foreach (var config in configs)
            {
                if (config == null)
                    continue;

                RegisterInstance(config);
            }
        }

        private void RegisterFallbackConfigs()
        {
            if (!Builder.Exists(typeof(ILoggerParameters), true))
            {
                Register<DefaultLoggerParameters>(Lifetime.Singleton);
            }
        }

        private void RegisterFactories()
        {
            Register<ViewsFactory>(Lifetime.Singleton);
            Register<ViewModelFactory>(Lifetime.Singleton);
            RegisterEntryPoint<ScreensFactory>();
        }

        private void RegisterServices()
        {
            Register<SceneResources>(Lifetime.Singleton);
            Register<SceneService>(Lifetime.Singleton);
            Register<ScreenService>(Lifetime.Singleton);
            Register<PoolService>(Lifetime.Singleton);
            Register<ChatService>(Lifetime.Singleton);
            Register<LoggerService>(Lifetime.Singleton);
        }

        private void RegisterEntryPoints()
        {
            RegisterEntryPoint<WebsocketConnectionService>();
        }
    }
}
