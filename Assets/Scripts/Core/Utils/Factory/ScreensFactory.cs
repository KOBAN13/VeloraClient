using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utils.Addressable;
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
        
        public IReadOnlyList<Type> ScreenTypes => _screensData.Screens
            .Select(screen => screen.Type)
            .ToArray();

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
            return (TView)await CreateAsync(typeof(TView));
        }

        public async UniTask<View> CreateAsync(Type viewType)
        {
            var data = FindData(viewType);
            var prefabObject = await data.Asset.LoadAssetAsync<GameObject>();

            _sceneResources.AddObjectToRelease(prefabObject);
            var prefab = prefabObject.GetComponent(viewType) as View;
            
            var screen = _viewsFactory.Create(prefab, _uiRootService.Root);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            var data = FindData(typeof(TView));
            var handle = data.Asset.LoadAssetAsync<GameObject>();
            var obj = handle.WaitForCompletion();
            _sceneResources.AddObjectToRelease(obj);
            var prefab = obj.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _uiRootService.Root);
            screen.gameObject.SetActive(false);
            return screen;
        }

        private AddressablePrefabByType<View> FindData(Type viewType)
        {
            var data = _screensData.Screens
                .FirstOrDefault(d => d.Type == viewType);

            return data;
        }
    }
}
