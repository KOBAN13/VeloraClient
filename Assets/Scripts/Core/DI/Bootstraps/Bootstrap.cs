using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Core.Utils.SceneManagement.Interfaces;
using Core.Utils.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.DI.Bootstraps
{
    public abstract class Bootstrap<TLifetimeScope> : MonoBehaviour where TLifetimeScope : BaseLifeTimeScope
    {
        [SerializeField] protected TLifetimeScope LifetimeScope;
        [Inject] protected IObjectResolver ObjectResolver;

        [Inject] private TickService _tickService;
        private HashSet<IInitializable> _initializables = new();
        private HashSet<IScenesService> _sceneChangeds = new();
        private HashSet<ITickable> _tickableServices = new();
        private List<ITickable> _tickableBehaviours = new();
        private bool _isTicked;

        private async void Awake()
        {
            LifetimeScope.Build();

            await Initialize();

            _isTicked = true;
        }

        protected void InitializeServices()
        {
            _initializables = ObjectResolver.Resolve<IEnumerable<IInitializable>>().ToHashSet();
            _tickableServices = ObjectResolver.Resolve<IEnumerable<ITickable>>().ToHashSet();
            _tickService = ObjectResolver.Resolve<TickService>();

            foreach (var initializable in _initializables)
            {
                if (!initializable.IsInitialized)
                {
                    initializable.Initialize();
                    initializable.IsInitialized = true;
                }
            }

            foreach (var sceneInitializable in _sceneChangeds)
            {
                //sceneInitializable.SceneChanged();
            }

            foreach (var tickable in _tickableServices)
            {
                _tickService.RegisterTick(tickable);
            }
        }

        private void Update()
        {
            if (!_isTicked)
                return;

            _tickService.Tick(Time.deltaTime);
            Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            foreach (var tickable in _tickableServices)
            {
                _tickService.UnregisterTick(tickable);
            }

            foreach (var tickableBehaviour in _tickableBehaviours)
            {
                _tickService.UnregisterTick(tickableBehaviour);
            }

            Dispose();
        }

        private void Reset()
        {
            LifetimeScope ??= GetComponent<TLifetimeScope>();
        }

        protected virtual UniTask Initialize() => UniTask.CompletedTask;

        protected virtual void Tick(float deltaTime)
        {
        }

        protected virtual void Dispose()
        {
        }
    }
}