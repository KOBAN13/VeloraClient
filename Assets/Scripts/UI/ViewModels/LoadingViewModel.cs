using Core.Utils.SceneManagement;
using R3;
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
        public readonly ViewModelBinder<float> ProgressBarBinder = new();
        
        public override void Initialize()
        {
            _sceneLoader.Progress
                .Subscribe(value => ProgressBarBinder.Value = value)
                .AddTo(Disposable);
        }
    }
}
