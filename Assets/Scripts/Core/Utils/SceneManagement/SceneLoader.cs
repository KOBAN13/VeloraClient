using System;
using Core.Utils.Factory;
using Core.Utils.SceneManagement.Interfaces;
using Core.Utils.Screens;
using Cysharp.Threading.Tasks;
using R3;
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
        [SerializeField, Range(0.1f, 0.95f)] private float _sceneProgressLimit = 0.85f;

        private IScenesService _scenesService;
        private IScreenService _screenService;
        private SceneScopeResolver _sceneScopeResolver;
        private float _targetProgress;
        
        public readonly ReactiveProperty<bool> IsLoading = new();
        public readonly ReactiveProperty<float> Progress = new();
        
        [Inject]
        private void Construct(
            SceneResources sceneResources,
            IScenesService scenesService,
            IScreenService screenService,
            SceneScopeResolver sceneScopeResolver)
        {
            _scenesService = scenesService;
            _screenService = screenService;
            _sceneScopeResolver = sceneScopeResolver;
            _scenesService.Construct(this, sceneResources);
        }
        
        public async UniTask LoadScene(TypeScene typeScene, params Type[] screensToPreload)
        {
            ChangeParameters();
            
            _scenesService.UnloadResources();
            
            _screenService.ClearCollections();
            
            var loadingScene = await _scenesService.LoadLoadingScene(_sceneGroup);
            
            await _screenService.OpenAsync<LoadingScreen>();
            
            IsLoading.Value = true;
            await UniTask.Yield();

            var sceneProgress = new LoadingProgress(_minSlider, _sceneProgressLimit);
            var screensProgress = new LoadingProgress(_sceneProgressLimit, 1f);
            sceneProgress.Progressed += SetTargetProgress;
            screensProgress.Progressed += SetTargetProgress;
            
            await _scenesService.UnloadScene();
            
            var loadedScene = await _scenesService.LoadScene(_sceneGroup, sceneProgress, typeScene);
            _sceneScopeResolver.ResolveRequired(loadedScene);
            await _screenService.PreloadAsync(screensProgress, screensToPreload);
            
            SetTargetProgress(1f);
            Progress.Value = 1f;
            await UniTask.Yield();

            _screenService.CloseScreen<LoadingScreen>();
            
            await Addressables.UnloadSceneAsync(loadingScene);
            IsLoading.Value = false;
        }

        private void Update()
        {
            if(!IsLoading.Value) 
                return;

            Progress.Value = _targetProgress;
        }

        private void ChangeParameters()
        {
            Progress.Value = _minSlider;
            _targetProgress = _minSlider;
        }

        private void SetTargetProgress(float value)
        {
            _targetProgress = Mathf.Clamp01(value);
        }
    }
}
