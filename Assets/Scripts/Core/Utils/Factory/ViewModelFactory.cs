using UI.Core;

namespace Core.Utils.Factory
{
    public class ViewModelFactory : Base.Factory
    {
        private readonly SceneScopeResolver _sceneScopeResolver;

        public ViewModelFactory(SceneScopeResolver sceneScopeResolver)
        {
            _sceneScopeResolver = sceneScopeResolver;
        }
        
        public T Create<T>() where T : ViewModel, new()
        {
            var viewModel = new T();
            _sceneScopeResolver.Resolve().Inject(viewModel);
            return viewModel;
        }
    }
}
