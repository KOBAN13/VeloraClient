using System;
using System.Collections.Generic;
using Core.Utils.Factory;
using Core.Utils.Logger;
using Core.Utils.Pool;
using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Core.Utils.Services;
using Core.Utils.StateMachine.Project;
using Core.Utils.StateMachine.Project.Factory;
using Core.Utils.StateMachine.Project.States;
using Cysharp.Threading.Tasks;
using Network.Messaging;
using Network.Services.Auth;
using Network.Services.Identity;
using Network.Services.Lobby;
using Network.Services.Match;
using Network.Transport;
using Network.Transport.Client;
using Network.Transport.Codecs;
using Network.Transport.Framing;
using Network.Transport.WebSocket;
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

        private readonly List<ScriptableObject> _configs = new();
        private AsyncOperationHandle<IList<ScriptableObject>> _configsHandle;
        
        public async UniTask LoadConfigsAsync()
        {
            _configsHandle = Addressables.LoadAssetsAsync<ScriptableObject>(_configLabel, null);

            await _configsHandle;

            if (_configsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to load configs by label '{_configLabel.labelString}'.");
            }

            _configs.Clear();
            _configs.AddRange(_configsHandle.Result);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            
            RegisterComponents();
            RegisterConfigs();
            RegisterFactories();
            RegisterRootServices();
            RegisterNetworkServices();
            RegisterLobbyServices();
            RegisterProjectStates();
        }

        private void RegisterComponents()
        {
            RegisterComponent(_sceneLoader);
        }

        private void RegisterConfigs()
        {
            foreach (var config in _configs)
            {
                RegisterInstance(config);
            }
        }

        protected override void OnDestroy()
        {
            if (_configsHandle.IsValid())
            {
                Addressables.Release(_configsHandle);
            }

            base.OnDestroy();
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
            Register<LoginClientService>(Lifetime.Singleton);
            RegisterEntryPoint<MatchStartCoordinator>();
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

        private void RegisterLobbyServices()
        {
            RegisterEntryPoint<LobbyClientService>();
            RegisterEntryPoint<RoomStateService>();
            Register<GameListItemPool>(Lifetime.Singleton);
            Register<PlayerLobbyItemPool>(Lifetime.Singleton);
        }

        private void RegisterProjectStates()
        {
            Register<ProjectAStateMachine>(Lifetime.Singleton);

            Register<ProjectBootstrapState>(Lifetime.Singleton);
            Register<ProjectMainMenu>(Lifetime.Singleton);
            Register<ProjectLobbyState>(Lifetime.Singleton);
            Register<ProjectGameState>(Lifetime.Singleton);
        }
    }
}
