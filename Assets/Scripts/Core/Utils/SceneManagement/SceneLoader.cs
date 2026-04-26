using System;
using Core.Utils.SceneManagement.Interfaces;
using Core.Utils.Screens;
using Cysharp.Threading.Tasks;
using R3;
using Services.SceneManagement;
using Services.SceneManagement.Enums;
using UI.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Core.Utils.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneGroup _sceneGroup;
        [SerializeField] private float _minSlider = 0.05f;

        private IScenesService _scenesService;
        private IScreenService _screenService;
        private float _targetProgress;
        private IDisposable _sceneLoadedSubscription;
        
        private readonly LoadingProgress _loadingProgress = new();
        
        public readonly ReactiveProperty<bool> IsLoading = new();
        public readonly ReactiveProperty<float> Progress = new();
        
        [Inject]
        private void Construct(SceneResources sceneResources, IScenesService scenesService, IScreenService screenService)
        {
            _scenesService = scenesService;
            _screenService = screenService;
            _scenesService.Construct(this, sceneResources);
            _sceneLoadedSubscription = _scenesService.SceneIsLoad.Subscribe(_ => _screenService.Close());
        }
        
        public async UniTask LoadScene(TypeScene typeScene)
        {
            ChangeParameters();
            
            _scenesService.UnloadResources();
            
            _screenService.ClearCollections();
            
            var loadingScene = await _scenesService.LoadLoadingScene(_sceneGroup);
            
            var loadingScreen = await _screenService.OpenAsync<LoadingScreen>();
            
            _loadingProgress.Progressed += value => _targetProgress = value;
            IsLoading.Value = true;
            
            await _scenesService.UnloadScene();
            
            await _scenesService.LoadScene(_sceneGroup, _loadingProgress, typeScene);
            
            loadingScreen.Dispose();
            
            await Addressables.UnloadSceneAsync(loadingScene);
            
            _scenesService.UnloadResources();
        }
        
        private void OnDestroy()
        {
            _sceneLoadedSubscription?.Dispose();
        }

        private void Update()
        {
            if(IsLoading.Value == false) 
                return;

            Progress.Value = _targetProgress;
        }

        private void ChangeParameters()
        {
            Progress.Value = _minSlider;
        }
    }
}
