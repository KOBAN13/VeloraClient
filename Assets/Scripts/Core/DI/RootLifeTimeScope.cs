using System.Linq;
using Core.Utils.Factory;
using Core.Utils.Logger;
using Core.Utils.Pool;
using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Core.Utils.Services;
using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.Factory;
using Core.Utils.StateMachine.Project.States;
using Factories;
using Network;
using Network.Transport;
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
                RegisterInstance(config);
            }
        }

        private void RegisterFactories()
        {
            Register<ViewsFactory>(Lifetime.Singleton);
            Register<ViewModelFactory>(Lifetime.Singleton);
            Register<ProjectAStateFactory>(Lifetime.Singleton);
            RegisterEntryPoint<ScreensFactory>();
        }

        private void RegisterServices()
        {
            Register<UiRootService>(Lifetime.Singleton);
            Register<SceneResources>(Lifetime.Singleton);
            Register<SceneService>(Lifetime.Singleton);
            Register<ScreenService>(Lifetime.Singleton);
            Register<TickService>(Lifetime.Singleton);
            Register<PoolService>(Lifetime.Singleton);
            Register<ChatService>(Lifetime.Singleton);
            Register<LoggerService>(Lifetime.Singleton);
            Register<LoginClientService>(Lifetime.Singleton);
            Register<WebSocketTransport>(Lifetime.Singleton);
            Register<ProtobufPacketCodec>(Lifetime.Singleton);
            Register<WebSocketMessageFramer>(Lifetime.Singleton);
            Register<NetworkClient>(Lifetime.Singleton);
            RegisterStates();
        }

        private void RegisterStates()
        {
            Register<ProjectAStateMachine>(Lifetime.Singleton);

            Register<ProjectBootstrapState>(Lifetime.Singleton);
            Register<ProjectMainMenu>(Lifetime.Singleton);
            Register<ProjectGameState>(Lifetime.Singleton);
        }

        private void RegisterEntryPoints()
        {
        }
    }
}
