using UI.Core;
using VContainer;

namespace Core.Utils.Factory
{
    public class ViewModelFactory : Base.Factory
    {
        private readonly IObjectResolver _resolver;

        public ViewModelFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        public T Create<T>() where T : ViewModel, new()
        {
            var viewModel = new T();
            _resolver.Inject(viewModel);
            return viewModel;
        }
    }
}
