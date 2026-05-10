using UI.Core;
using UnityEngine;

namespace Core.Utils.Factory
{
    public class ViewsFactory : Base.Factory
    {
        private readonly SceneScopeResolver _sceneScopeResolver;
        
        public ViewsFactory(SceneScopeResolver sceneScopeResolver)
        {
            _sceneScopeResolver = sceneScopeResolver;
        }
        
        public TView Create<TView>(TView prefab, Transform parent)
            where TView : View
        {
            var view = Object.Instantiate(prefab, parent);
            InitializeView(view);
            return view;
        }

        public View Create(View prefab, Transform parent)
        {
            var view = Object.Instantiate(prefab, parent);
            InitializeView(view);
            return view;
        }

        public void InitializeView(View view)
        {
            _sceneScopeResolver.Resolve().Inject(view);
            view.Initialize();
        }
    }
}
