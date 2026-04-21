using R3;
using Services.SceneManagement;
using UI.Core;
using UI.Helpers;
using VContainer;

namespace UI.ViewModels
{
    public class LoadingViewModel : ViewModel
    {
        [Inject] 
        private SceneLoader _sceneLoader;
        
        [AutoBind]
        private readonly ViewModelBinder<float> _progressBinder = new();
        
        public override void Initialize()
        {
            _sceneLoader.Progress.Skip(1)
                .Subscribe(value => _progressBinder.Value = value)
                .AddTo(Disposable);
        }
    }
}
