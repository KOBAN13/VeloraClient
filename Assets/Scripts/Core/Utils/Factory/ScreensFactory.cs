using System.Linq;
using Core.Utils.Data;
using Cysharp.Threading.Tasks;
using Factories;
using Services.SceneManagement;
using UI.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Utils.Factory
{
    public class ScreensFactory : Base.Factory, IInitializable
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensData _screensData;
        [Inject] private SceneResources _sceneResources;
        
        private Transform _parent;
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            if (_screensData.Root != null)
                _parent = Object.Instantiate(_screensData.Root, null).transform;
        }

        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            Initialize();

            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = await data.Asset.LoadAssetAsync<GameObject>();
            _sceneResources.AddObjectToRelease(handle);
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _parent);
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
            var screen = _viewsFactory.Create(prefab, _parent);
            screen.gameObject.SetActive(false);
            return screen;
        }
    }
}
