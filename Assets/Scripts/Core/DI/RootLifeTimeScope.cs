using System;
using System.Linq;
using Core.Utils.Factory;
using Core.Utils.Logger;
using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Core.Utils.Services;
using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.Factory;
using Core.Utils.StateMachine.Project.States;
using Cysharp.Threading.Tasks;
using Network.Messaging;
using Network.Services.Identity;
using Network.Transport;
using Network.Transport.Client;
using Network.Transport.Codecs;
using Network.Transport.Framing;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
            RegisterRootServices();
            RegisterNetworkServices();
            RegisterProjectStates();
        }

        private void RegisterComponents()
        {
            RegisterComponent(_sceneLoader);
        }

        private async void RegisterConfigs()
        {
            try
            {
                var configsHandle = Addressables.LoadAssetsAsync<ScriptableObject>(_configLabel, null);

                await configsHandle;

                if (configsHandle.Status != AsyncOperationStatus.Succeeded) 
                    return;
            
                foreach (var config in configsHandle.Result)
                {
                    RegisterInstance(config);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void RegisterFactories()
        {
            Register<SceneScopeResolver>(Lifetime.Singleton);
            Register<ViewsFactory>(Lifetime.Singleton);
            Register<ViewModelFactory>(Lifetime.Singleton);
            Register<ProjectAStateFactory>(Lifetime.Singleton);
            RegisterEntryPoint<ScreensFactory>();
        }

        private void RegisterRootServices()
        {
            Register<UiRootService>(Lifetime.Singleton);
            Register<SceneResources>(Lifetime.Singleton);
            Register<SceneService>(Lifetime.Singleton);
            Register<ScreenService>(Lifetime.Singleton);
            Register<TickService>(Lifetime.Singleton);
            Register<LoggerService>(Lifetime.Singleton);
        }

        private void RegisterNetworkServices()
        {
            Register<NetworkMessageBus>(Lifetime.Singleton);
            Register<WebSocketTransport>(Lifetime.Singleton);
            Register<ProtobufPacketCodec>(Lifetime.Singleton);
            Register<WebSocketMessageFramer>(Lifetime.Singleton);
            Register<NetworkClient>(Lifetime.Singleton);
            Register<ClientIdentityService>(Lifetime.Singleton);
        }

        private void RegisterProjectStates()
        {
            Register<ProjectAStateMachine>(Lifetime.Singleton);

            Register<ProjectBootstrapState>(Lifetime.Singleton);
            Register<ProjectMainMenu>(Lifetime.Singleton);
            Register<ProjectGameState>(Lifetime.Singleton);
        }
    }
}
