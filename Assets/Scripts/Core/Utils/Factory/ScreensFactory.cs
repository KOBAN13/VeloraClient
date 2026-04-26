using System.Linq;
using Core.Utils.Data;
using Core.Utils.SceneManagement;
using Core.Utils.Services;
using Cysharp.Threading.Tasks;
using Factories;
using Services.SceneManagement;
using UI.Core;
using UnityEngine;
using VContainer;

namespace Core.Utils.Factory
{
    public class ScreensFactory : Base.Factory, IInitializable
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensData _screensData;
        [Inject] private SceneResources _sceneResources;
        
        private Canvas _canvas;

        public bool IsInitialized { get; set; }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            _canvas = Object.Instantiate(_screensData.Canvas, null);
        }

        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            Initialize();

            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            
            var handle = await data.Asset.LoadAssetAsync<GameObject>();
            
            _sceneResources.AddObjectToRelease(handle);
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _canvas.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            Initialize();

            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = data.Asset.LoadAssetAsync<GameObject>();
            var obj = handle.WaitForCompletion();
            var prefab = obj.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _canvas.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }
    }
}
