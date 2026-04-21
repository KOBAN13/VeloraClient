using R3;
using Services.SceneManagement;
using UI.Base;
using UI.Helpers;
using VContainer;

namespace UI.ViewModel
{
    public class LoadingViewModel : Base.ViewModel
    {
        [Inject] 
        private SceneLoader _sceneLoader;
        
        [AutoBind]
        private readonly ViewModelBinder<float> _progressBinder = new();
        
        public override void Initialize()
        {
            Bind(_progressBinder);
            
            _sceneLoader.Progress.Skip(1)
                .Subscribe(value => _progressBinder.Value = value)
                .AddTo(Disposable);
        }
    }
}