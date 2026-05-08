using System;
using Cysharp.Threading.Tasks;
using UI.Core;

namespace Core.Utils.Screens
{
    public interface IScreenService
    {
        UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View;
        UniTask<TScreen> OpenAsync<TScreen, TPayload>(TPayload payload) where TScreen : View;
        UniTask PreloadAsync(IProgress<float> progress = null, params Type[] screenTypes);
        TScreen OpenSync<TScreen>() where TScreen : View;
        TScreen OpenSync<TScreen, TPayload>(TPayload payload) where TScreen : View;
        void ClearCollections();
        void CloseScreen<TScreen>() where TScreen : View;
        void Close();
    }
}
