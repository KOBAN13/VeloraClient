using Cysharp.Threading.Tasks;
using VContainer;

namespace Core.DI.Bootstraps
{
    public class StartBootstrup
    {
        [Inject] private ScreensService _screensService;
        
        private async UniTask Initialize()
        {
            await _screensService.OpenAsync<MainMenuScreen>();
        }
    }
}