using Core.Utils.Screens;
using Cysharp.Threading.Tasks;
using UI.Views;
using VContainer;

namespace Core.DI.Bootstraps
{
    public class StartBootstraps : Bootstrap<RootLifeTimeScope>
    {
        [Inject] private IScreenService _screensService;
        
        protected override async UniTask Initialize()
        {
            InitializeServices();
            
            DontDestroyOnLoad(gameObject);
            
            await _screensService.OpenAsync<ChatScreen>();
        }
    }
}