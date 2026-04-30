using System.Linq;
using Core.Utils.Data;
using Core.Utils.SceneManagement;
using Core.Utils.Screens;
using Cysharp.Threading.Tasks;
using Factories;
using UI.Core;
using UnityEngine;

namespace Core.Utils.Factory
{
    public class ScreensFactory : Base.Factory
    {
        private readonly ViewsFactory _viewsFactory;
        private readonly ScreensData _screensData;
        private readonly SceneResources _sceneResources;
        
        private readonly IUiRootService _uiRootService;

        public ScreensFactory(
            IUiRootService uiRootService, 
            SceneResources sceneResources, 
            ScreensData screensData, 
            ViewsFactory viewsFactory)
        {
            _uiRootService = uiRootService;
            _sceneResources = sceneResources;
            _screensData = screensData;
            _viewsFactory = viewsFactory;
        }

        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            
            var handle = await data.Asset.LoadAssetAsync<GameObject>();
            
            _sceneResources.AddObjectToRelease(handle);
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _uiRootService.Root);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = data.Asset.LoadAssetAsync<GameObject>();
            var obj = handle.WaitForCompletion();
            var prefab = obj.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _uiRootService.Root);
            screen.gameObject.SetActive(false);
            return screen;
        }
    }
}
