using System;
using Core.Utils.SceneManagement.Interfaces;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using R3;
using Services.SceneManagement.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.Utils.SceneManagement
{
    public class SceneService : IScenesService
    {
        private SceneResources _resources;
        private AsyncOperationHandle<SceneInstance> _currentSceneHandle;
        private Scene _currentRegularScene;
        private readonly Subject<Unit> _sceneIsLoadSubject = new();

        public Observable<Unit> SceneIsLoad => _sceneIsLoadSubject;

        public void Construct(SceneLoader sceneLoader, SceneResources resources)
        {
            _resources = resources;
        }

        public async UniTask<Scene> LoadScene(SceneGroup sceneGroup, IProgress<float> progress, TypeScene typeScene)
        {
            var sceneRef = sceneGroup.FindSceneByReference(typeScene);

            switch (sceneRef.State)
            {
                case SceneReferenceState.Regular:
                    return await LoadRegularScene(sceneRef, sceneGroup, progress);
                case SceneReferenceState.Addressable:
                    return await LoadAddressableScene(sceneRef.Path, sceneGroup, progress);
                default:
                    Debug.LogError($"[SceneService] Scene {typeScene} is not safe to load: {sceneRef.UnsafeReason}");
                    return default;
            }
        }

        public async UniTask<AsyncOperationHandle<SceneInstance>> LoadLoadingScene(SceneGroup sceneGroup)
        {
            var scene = sceneGroup.FindSceneByReference(TypeScene.Loading);
            
            var sceneLoadOperation = Addressables.LoadSceneAsync(
                scene.Path,
                LoadSceneMode.Additive,
                false
            );
            
            await sceneLoadOperation.Task;
            
            if (sceneLoadOperation.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load scene: " + scene.Path);
                return sceneLoadOperation;
            }

            var sceneInstance = sceneLoadOperation.Result;

            if (!sceneInstance.Scene.IsValid())
            {
                Debug.LogWarning("Invalid scene instance loaded.");
            }

            await sceneInstance.ActivateAsync();

            SceneManager.SetActiveScene(sceneInstance.Scene); 
            return sceneLoadOperation;
        }

        public async UniTask UnloadScene()
        {
            if (_currentSceneHandle.IsValid())
            {
                await Addressables.UnloadSceneAsync(_currentSceneHandle);
                
                _currentSceneHandle = default;
                
                await Resources.UnloadUnusedAssets();
            }

            if (_currentRegularScene.IsValid() && _currentRegularScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(_currentRegularScene);

                _currentRegularScene = default;

                await Resources.UnloadUnusedAssets();
            }
        }

        public void UnloadResources()
        {
            foreach (var resource in _resources.ObjectToRelease)
                Addressables.Release(resource);
            
            _resources.ClearObject();
        }

        private async UniTask<Scene> LoadRegularScene(SceneReference sceneRef, SceneGroup sceneGroup, IProgress<float> progress)
        {
            var initSceneName = sceneGroup.FindSceneByReference(TypeScene.InitialScene).Name;

            var initScene = SceneManager.GetSceneByName(initSceneName);
            var hadInit = initScene.IsValid() && initScene.isLoaded;

            var operation = SceneManager.LoadSceneAsync(sceneRef.Path, LoadSceneMode.Additive);

            if (operation == null)
            {
                Debug.LogError($"[SceneService] Failed to load scene: {sceneRef.Path}");
                return default;
            }

            while (!operation.isDone)
            {
                progress.Report(Mathf.Clamp01(operation.progress));
                await UniTask.Yield();
            }

            var loadedScene = SceneManager.GetSceneByPath(sceneRef.Path);

            if (!loadedScene.IsValid())
            {
                loadedScene = SceneManager.GetSceneByName(sceneRef.Name);
            }

            if (!loadedScene.IsValid())
            {
                Debug.LogWarning("Invalid scene instance loaded.");
                return default;
            }

            SceneManager.SetActiveScene(loadedScene);

            progress.Report(1f);

            if (hadInit)
            {
                await SceneManager.UnloadSceneAsync(initScene);
            }

            _currentRegularScene = loadedScene;
            _sceneIsLoadSubject.OnNext(Unit.Default);

            return loadedScene;
        }

        private async UniTask<Scene> LoadAddressableScene(string scenePath, SceneGroup sceneGroup, IProgress<float> progress)
        {
            var initSceneName = sceneGroup.FindSceneByReference(TypeScene.InitialScene).Name;
            
            var initScene = SceneManager.GetSceneByName(initSceneName);
            var hadInit = initScene.IsValid() && initScene.isLoaded;

            var handle = Addressables.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Additive,
                false
                );
            
            _currentSceneHandle = handle;

            var fakeProgress = 0f;
            
            while (!handle.IsDone)
            {
                var target = Mathf.Clamp01(handle.PercentComplete);
                fakeProgress = Mathf.MoveTowards(fakeProgress, target, 0.01f);
                progress.Report(fakeProgress);

                await UniTask.Yield();
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"[SceneService] Failed to load scene: {scenePath}");
                return default;
            }
            
            var sceneInstance = handle.Result;
            if (!sceneInstance.Scene.IsValid())
            {
                Debug.LogWarning("Invalid scene instance loaded.");
                return default;
            }

            await sceneInstance.ActivateAsync();
            
            SceneManager.SetActiveScene(sceneInstance.Scene);

            progress.Report(1f);
            
            if (hadInit)
            {
                await SceneManager.UnloadSceneAsync(initScene);
            }

            _currentRegularScene = default;
            _sceneIsLoadSubject.OnNext(Unit.Default);
            
            return sceneInstance.Scene;
        }
    }
}
