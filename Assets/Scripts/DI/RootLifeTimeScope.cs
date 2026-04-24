using System.Linq;
using Network;
using Services.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace DI
{
    public class RootLifeTimeScope : BaseLifeTimeScope
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private AssetLabelReference _configLable;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            
            RegisterConfigs();
            RegisterFactories();
            RegisterServices();
        }

        private void RegisterConfigs()
        {
            var configs = Addressables
                .LoadAssetsAsync<ScriptableObject>(_configLable, null)
                .WaitForCompletion()
                .ToList();

            foreach (var config in configs)
            {
                RegisterInstance(config);
            }
        }

        private void RegisterFactories()
        {
            
        }

        private void RegisterServices()
        {
            Register<WebsocketConnectionService>(Lifetime.Singleton);
        }
    }
}